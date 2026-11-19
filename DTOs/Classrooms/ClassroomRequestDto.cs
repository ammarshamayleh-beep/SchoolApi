using SchoolApi.Validators.Classrooms;

namespace SchoolApi.DTOs.Classrooms;

// Request DTO: what comes IN, for both POST and PUT (same fields)
// The rules are in Validators/Classrooms/ClassroomRequestDtoValidator.cs
public class ClassroomRequestDto
{
    public string Name { get; set; } = "";
    public int Year { get; set; }
}
