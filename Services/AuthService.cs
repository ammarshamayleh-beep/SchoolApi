using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Auth;
using SchoolApi.Data;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Auth;
using SchoolApi.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using SchoolApi.Models;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Services;

public class AuthService : IAuthService
{
    private readonly SchoolContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;
    private readonly IMapper _mapper;

    public AuthService(
        SchoolContext context,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService,
        ILogger<AuthService> logger,
        IMapper mapper)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<BaseResponse<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        // Same answer for a wrong email and a wrong password,
        // so nobody can find out which emails exist
        if (user == null ||
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Failed login for {Email}", dto.Email);
            return BaseResponse<AuthResponseDto>.Failure(StatusCodes.Status401Unauthorized, "Invalid email or password.");
        }

        _logger.LogInformation("User {UserId} logged in as {Role}", user.Id, user.Role);

        var response = new AuthResponseDto
        {
            Token = _tokenService.CreateToken(user),
            Email = user.Email,
            Role = user.Role
        };

        return BaseResponse<AuthResponseDto>.Success(response);
    }

    // Database trips: 2 (one query for the teacher or student check + the INSERT), or 1 for an Admin account
    public async Task<BaseResponse<UserDto>> RegisterAsync(RegisterDto dto)
    {
        var user = new User
        {
            Email = dto.Email,
            Role = dto.Role
        };

        // Rule 1: a teacher account points to a real teacher, who has no account yet
        if (dto.Role == Roles.Teacher)
        {
            if (dto.TeacherId == null)
            {
                return BaseResponse<UserDto>.Failure(StatusCodes.Status400BadRequest, "TeacherId is required for a Teacher account.");
            }

            // One query: does the teacher exist, and does he already have an account?
            var teacher = await _context.Teachers
                .Where(t => t.Id == dto.TeacherId)
                .Select(t => new { HasAccount = _context.Users.Any(u => u.TeacherId == t.Id) })
                .FirstOrDefaultAsync();

            if (teacher == null)
            {
                return BaseResponse<UserDto>.Failure(StatusCodes.Status400BadRequest, $"Teacher {dto.TeacherId} was not found.");
            }

            if (teacher.HasAccount)
            {
                return BaseResponse<UserDto>.Failure(StatusCodes.Status409Conflict, $"Teacher {dto.TeacherId} already has an account.");
            }

            user.TeacherId = dto.TeacherId;
        }

        // Rule 2: the same for a student account
        if (dto.Role == Roles.Student)
        {
            if (dto.StudentId == null)
            {
                return BaseResponse<UserDto>.Failure(StatusCodes.Status400BadRequest, "StudentId is required for a Student account.");
            }

            var student = await _context.Students
                .Where(s => s.Id == dto.StudentId)
                .Select(s => new { HasAccount = _context.Users.Any(u => u.StudentId == s.Id) })
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return BaseResponse<UserDto>.Failure(StatusCodes.Status400BadRequest, $"Student {dto.StudentId} was not found.");
            }

            if (student.HasAccount)
            {
                return BaseResponse<UserDto>.Failure(StatusCodes.Status409Conflict, $"Student {dto.StudentId} already has an account.");
            }

            user.StudentId = dto.StudentId;
        }

        // Save the HASH of the password, never the password itself
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            // Rule 3: the email is unique. The unique index checked it, so no AnyAsync before saving.
            return BaseResponse<UserDto>.Failure(StatusCodes.Status409Conflict, $"Email {dto.Email} is already used.");
        }

        _logger.LogInformation("Created {Role} account {UserId}", user.Role, user.Id);

        return BaseResponse<UserDto>.Success(_mapper.Map<UserDto>(user), StatusCodes.Status201Created);
    }
}
