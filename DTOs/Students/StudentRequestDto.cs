namespace SchoolApi.DTOs.Students;

// Request DTO: what comes IN, for both POST and PUT
public class StudentRequestDto
{
    
    public string FirstName { get; set; } = "";

    
    public string LastName { get; set; } = "";


    public string Email { get; set; } = "";

    public DateTime DateOfBirth { get; set; }

    
    public Guid ClassroomId { get; set; }
}
