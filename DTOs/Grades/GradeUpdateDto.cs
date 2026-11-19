namespace SchoolApi.DTOs.Grades;

// Request DTO for PUT: fixing a grade only changes the score
public class GradeUpdateDto
{
    public decimal Score { get; set; }
}
