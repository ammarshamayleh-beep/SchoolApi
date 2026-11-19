namespace SchoolApi.DTOs.Reports;

// How a classroom went
public class ClassroomAverageDto
{
    public Guid ClassroomId { get; set; }
    public string ClassroomName { get; set; } = "";
    public int StudentCount { get; set; }
    public decimal Average { get; set; }
}
