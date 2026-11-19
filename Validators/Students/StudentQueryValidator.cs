using FluentValidation;
using SchoolApi.DTOs.Students;
using SchoolApi.Validators.Common;

namespace SchoolApi.Validators.Students;

public class StudentQueryValidator : AbstractValidator<StudentQuery>
{
    public StudentQueryValidator()
    {
        // Reuse the page rules from PaginationQueryValidator
        Include(new PaginationQueryValidator());
    }
}
