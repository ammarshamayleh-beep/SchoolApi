namespace SchoolApi.DTOs.Reports;

// One student's report card
public class StudentReportDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public string ClassroomName { get; set; } = "";
    public string? Term { get; set; }            // null = all terms
    public decimal? Average { get; set; }        // null = no grades yet
    public List<SubjectScoreDto> Grades { get; set; } = new();
}
