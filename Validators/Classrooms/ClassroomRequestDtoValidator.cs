using FluentValidation;
using SchoolApi.DTOs.Classrooms;

namespace SchoolApi.Validators.Classrooms;

public class ClassroomRequestDtoValidator : AbstractValidator<ClassroomRequestDto>
{
    public ClassroomRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Year)
            .InclusiveBetween(2000, 2100);
    }
}
