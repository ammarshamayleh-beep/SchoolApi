using FluentValidation;
using SchoolApi.Options;

namespace SchoolApi.Validators.Options;

// The rules for the "Jwt" settings, checked when the app starts
public class JwtOptionsValidator : AbstractValidator<JwtOptions>
{
    public JwtOptionsValidator()
    {
        // HMAC-SHA256 needs a key of at least 32 characters
        RuleFor(x => x.Key)
            .NotEmpty()
            .MinimumLength(32);

        RuleFor(x => x.Issuer)
            .NotEmpty();

        RuleFor(x => x.Audience)
            .NotEmpty();

        // From 1 minute to 1 day
        RuleFor(x => x.ExpiresInMinutes)
            .InclusiveBetween(1, 1440);
    }
}
