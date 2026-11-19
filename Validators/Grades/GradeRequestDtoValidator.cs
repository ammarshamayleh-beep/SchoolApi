using FluentValidation;
using SchoolApi.DTOs.Grades;

namespace SchoolApi.Validators.Grades;

public class GradeRequestDtoValidator : AbstractValidator<GradeRequestDto>
{
    public GradeRequestDtoValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.SubjectId).NotEmpty();
        RuleFor(x => x.Term).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Score).InclusiveBetween(0, 100);
    }
}
