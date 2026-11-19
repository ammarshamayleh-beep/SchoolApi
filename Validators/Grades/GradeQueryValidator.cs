using FluentValidation;
using SchoolApi.DTOs.Grades;
using SchoolApi.Validators.Common;

namespace SchoolApi.Validators.Grades;

public class GradeQueryValidator : AbstractValidator<GradeQuery>
{
    public GradeQueryValidator()
    {
        // Reuse the page rules from PaginationQueryValidator
        Include(new PaginationQueryValidator());
    }
}
