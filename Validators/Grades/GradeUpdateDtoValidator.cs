using FluentValidation;
using SchoolApi.DTOs.Grades;

namespace SchoolApi.Validators.Grades;

public class GradeUpdateDtoValidator : AbstractValidator<GradeUpdateDto>
{
    public GradeUpdateDtoValidator()
    {
        RuleFor(x => x.Score).InclusiveBetween(0, 100);
    }
}
