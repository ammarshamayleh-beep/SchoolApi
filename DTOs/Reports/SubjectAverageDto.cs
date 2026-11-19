namespace SchoolApi.DTOs.Reports;

// How a subject went: average, lowest and highest score
public class SubjectAverageDto
{
    public Guid SubjectId { get; set; }
    public string SubjectCode { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public int GradeCount { get; set; }
    public decimal Average { get; set; }
    public decimal Lowest { get; set; }
    public decimal Highest { get; set; }
}
