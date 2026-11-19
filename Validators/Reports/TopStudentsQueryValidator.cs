using FluentValidation;
using SchoolApi.DTOs.Reports;

namespace SchoolApi.Validators.Reports;

public class TopStudentsQueryValidator : AbstractValidator<TopStudentsQuery>
{
    public TopStudentsQueryValidator()
    {
        RuleFor(x => x.Count)
            .InclusiveBetween(1, 50);
    }
}
