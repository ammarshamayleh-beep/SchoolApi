using FluentValidation;
using SchoolApi.Auth;
using SchoolApi.DTOs.Auth;

namespace SchoolApi.Validators.Auth;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    private static readonly string[] AllowedRoles = { Roles.Admin, Roles.Teacher, Roles.Student };

    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);

        RuleFor(x => x.Role)
            .NotEmpty()
            .Must(role => AllowedRoles.Contains(role))
            .WithMessage("Role must be Admin, Teacher or Student.");

        // A Teacher account must point to a teacher
        RuleFor(x => x.TeacherId)
            .NotEmpty()
            .When(x => x.Role == Roles.Teacher)
            .WithMessage("TeacherId is required for a Teacher account.");

        // A Student account must point to a student
        RuleFor(x => x.StudentId)
            .NotEmpty()
            .When(x => x.Role == Roles.Student)
            .WithMessage("StudentId is required for a Student account.");
    }
}
