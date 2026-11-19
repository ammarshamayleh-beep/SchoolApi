using FluentValidation;
using SchoolApi.DTOs.Students;

namespace SchoolApi.Validators.Students;

public class StudentRequestDtoValidator : AbstractValidator<StudentRequestDto>
{
    public StudentRequestDtoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);

        // A date of birth is required, and it can't be in the future
        RuleFor(x => x.DateOfBirth).NotEmpty().LessThan(DateTime.Today);

        // NotEmpty on a Guid means: not 00000000-0000-0000-0000-000000000000
        RuleFor(x => x.ClassroomId).NotEmpty();
    }
}
