namespace SchoolApi.DTOs.Classrooms;

// Response DTO: what goes OUT to the client
public class ClassroomDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int Year { get; set; }
    public int StudentCount { get; set; }
}
