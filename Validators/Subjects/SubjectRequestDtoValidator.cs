using FluentValidation;
using SchoolApi.DTOs.Subjects;

namespace SchoolApi.Validators.Subjects;

public class SubjectRequestDtoValidator : AbstractValidator<SubjectRequestDto>
{
    public SubjectRequestDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);

        // TeacherId is optional, but if it is sent, it can't be an empty Guid
        RuleFor(x => x.TeacherId)
            .NotEqual(Guid.Empty)
            .When(x => x.TeacherId != null);
    }
}
