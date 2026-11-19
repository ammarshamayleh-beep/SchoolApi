namespace SchoolApi.DTOs.Reports;

// One line in the report card
public class SubjectScoreDto
{
    public string SubjectCode { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public string Term { get; set; } = "";
    public decimal Score { get; set; }
}
