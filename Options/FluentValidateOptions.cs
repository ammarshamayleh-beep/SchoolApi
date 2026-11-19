using FluentValidation;
using Microsoft.Extensions.Options;

namespace SchoolApi.Options;

// A bridge: lets the IOptions system check a settings class with its FluentValidation validator.
// IValidateOptions<T> is what ValidateOnStart() calls when the app starts.
public class FluentValidateOptions<TOptions> : IValidateOptions<TOptions> where TOptions : class
{
    private readonly IServiceScopeFactory _scopeFactory;

    public FluentValidateOptions(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        // The validators are registered as Scoped, so we open a scope to get one
        using var scope = _scopeFactory.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<IValidator<TOptions>>();

        var result = validator.Validate(options);

        if (result.IsValid)
        {
            return ValidateOptionsResult.Success;
        }

        var errors = result.Errors.Select(e => $"{typeof(TOptions).Name}.{e.PropertyName}: {e.ErrorMessage}");
        return ValidateOptionsResult.Fail(errors);
    }
}
