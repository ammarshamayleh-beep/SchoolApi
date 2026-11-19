namespace SchoolApi.DTOs.Subjects;

// Request DTO: what comes IN, for both POST and PUT
public class SubjectRequestDto
{
  
    public string Name { get; set; } = "";


    public string Code { get; set; } = "";

    public Guid? TeacherId { get; set; }
}
