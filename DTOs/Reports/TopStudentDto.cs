namespace SchoolApi.DTOs.Reports;

// One line in the top students list
public class TopStudentDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public string ClassroomName { get; set; } = "";
    public decimal Average { get; set; }
}
