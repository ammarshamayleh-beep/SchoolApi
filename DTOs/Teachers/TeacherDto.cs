namespace SchoolApi.DTOs.Teachers;

// Response DTO: what goes OUT to the client
public class TeacherDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime HireDate { get; set; }
    public int SubjectCount { get; set; }
}
