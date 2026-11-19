namespace SchoolApi.DTOs.Grades;

// Request DTO for POST: a new grade needs everything
public class GradeRequestDto
{
    
    public Guid StudentId { get; set; }

    public Guid SubjectId { get; set; }

   
    public string Term { get; set; } = "";

    public decimal Score { get; set; }
}
