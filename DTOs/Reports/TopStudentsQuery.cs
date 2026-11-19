using SchoolApi.Validators.Reports;

namespace SchoolApi.DTOs.Reports;

// GET api/reports/top-students?term=2026-T1&count=5
// The rules are in Validators/Reports/TopStudentsQueryValidator.cs
public class TopStudentsQuery
{
    public string? Term { get; set; }   // null = all terms
    public int Count { get; set; } = 5;
}
