namespace SchoolApi.DTOs.Subjects;

// Response DTO: what goes OUT to the client
public class SubjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Code { get; set; } = "";
    public Guid? TeacherId { get; set; }
    public string? TeacherName { get; set; }
}
