using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolApi.Auth;
using SchoolApi.Data;
using SchoolApi.Models;
using SchoolApi.Options;
using Microsoft.Extensions.Options;
using SchoolApi.Services;
using SchoolApi.Services.Abstractions;
using FluentValidation;
using SchoolApi.Filters;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.DTOs.Common;
using SchoolApi.Handlers;

var builder = WebApplication.CreateBuilder(args);

// The ValidationFilter runs the FluentValidation validators before every action
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    // Errors while reading the request (for example "year": "abc") also return a BaseResponse
    options.InvalidModelStateResponseFactory = context =>
    {
        var messages = context.ModelState.Values
            .SelectMany(value => value.Errors)
            .Select(error => error.ErrorMessage);

        return new BadRequestObjectResult(
            BaseResponse<object>.Failure(StatusCodes.Status400BadRequest, string.Join(" ", messages)));
    };
});

// Any exception nobody caught becomes a 500 BaseResponse
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// Finds every Profile class in this project (Mappings folder) and registers IMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));
// Finds every validator in this project and registers it
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
// Who is logged in? SchoolContext needs it for the audit columns
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Our services (Scoped: a new one for every request, like the DbContext)
builder.Services.AddScoped<IClassroomService, ClassroomService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// No data inside them, so one object for the whole app is enough (Singleton)
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

// IOptions: the "Jwt" section of appsettings.json becomes a JwtOptions object
// ValidateOnStart: a missing or wrong value stops the app at startup, not at the first login
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateOnStart();

// Check JwtOptions with its FluentValidation rules (JwtOptionsValidator)
builder.Services.AddSingleton<IValidateOptions<JwtOptions>, FluentValidateOptions<JwtOptions>>();

// The JWT check below needs the values now (before the app is built), so we read the same class here
JwtOptions jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

// Authentication: how to check the JWT in every request
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.Key)),

            RoleClaimType = ClaimTypes.Role
        };
    });

// Authorization: who can do what ([Authorize] on the controllers)
builder.Services.AddAuthorization();

// CORS: which websites are allowed to call this API from the browser
string[] allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi(options =>
{
    // Adds the "Authorize" button to Swagger
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

// Fill the database with starting data (on my machine only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SchoolContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
    await DbSeeder.SeedAsync(context, passwordHasher);
}
app.UseExceptionHandler();   // 1. any crash becomes a 500 BaseResponse (GlobalExceptionHandler)

// 2. errors with an empty body (401, 403, unknown URL...) get a BaseResponse body
app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;

    string error = response.StatusCode switch
    {
        StatusCodes.Status401Unauthorized => "You must log in first.",
        StatusCodes.Status403Forbidden => "You are not allowed to do this.",
        StatusCodes.Status404NotFound => "This URL was not found.",
        StatusCodes.Status405MethodNotAllowed => "This HTTP method is not allowed here.",
        _ => "The request failed."
    };

    await response.WriteAsJsonAsync(BaseResponse<object>.Failure(response.StatusCode, error));
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "SchoolApi v1");
        options.EnablePersistAuthorization();   // keep the token after a page refresh
    });
}

app.UseHttpsRedirection();
app.UseCors("Frontend");     // 3. before auth, so the browser's check request isn't blocked
app.UseAuthentication();     // 4. who are you? (reads the token)
app.UseAuthorization();      // 5. what can you do? (checks [Authorize])
app.MapControllers();

app.Run();
