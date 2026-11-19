using FluentValidation;
using SchoolApi.DTOs.Teachers;

namespace SchoolApi.Validators.Teachers;

public class TeacherRequestDtoValidator : AbstractValidator<TeacherRequestDto>
{
    public TeacherRequestDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.HireDate).NotEmpty();
    }
}
