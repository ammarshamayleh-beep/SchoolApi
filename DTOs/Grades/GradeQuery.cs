using SchoolApi.DTOs.Common;

namespace SchoolApi.DTOs.Grades;

// Filters from the URL: api/grades?studentId=1&subjectId=2&term=2026-T1
public class GradeQuery : PaginationQuery
{
    public Guid? StudentId { get; set; }
    public Guid? SubjectId { get; set; }
    public string? Term { get; set; }
}
