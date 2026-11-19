namespace SchoolApi.DTOs.Students;

// Response DTO: what goes OUT to the client
public class StudentDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public Guid ClassroomId { get; set; }
    public string ClassroomName { get; set; } = "";
}
