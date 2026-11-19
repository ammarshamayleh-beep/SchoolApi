

namespace SchoolApi.Models;

public class Student : BaseEntity
{
    
    public string FirstName { get; set; } = "";

    
    public string LastName { get; set; } = "";

   
    public string Email { get; set; } = "";

    public DateTime DateOfBirth { get; set; }
     
    // Foreign key: the classroom of this student
    public Guid ClassroomId { get; set; }
    public Classroom? Classroom { get; set; }
    // Many-to-Many with subjects, through grades
    public List<Grade> Grades { get; set; } = new();
}