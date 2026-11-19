namespace SchoolApi.DTOs.Grades;

// Response DTO: what goes OUT to the client
public class GradeDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public Guid SubjectId { get; set; }
    public string SubjectCode { get; set; } = "";
    public string Term { get; set; } = "";
    public decimal Score { get; set; }
}
