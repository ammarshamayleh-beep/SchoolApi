using FluentValidation;
using SchoolApi.DTOs.Common;

namespace SchoolApi.Validators.Common;

// The page rules. Other query validators reuse them with Include(...)
public class PaginationQueryValidator : AbstractValidator<PaginationQuery>
{
    public PaginationQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50);
    }
}
