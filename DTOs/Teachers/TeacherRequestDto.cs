namespace SchoolApi.DTOs.Teachers;

// Request DTO: what comes IN, for both POST and PUT
public class TeacherRequestDto
{
    
    public string FirstName { get; set; } = "";

   
    public string LastName { get; set; } = "";

  
    public string Email { get; set; } = "";

    public DateTime HireDate { get; set; }
}
