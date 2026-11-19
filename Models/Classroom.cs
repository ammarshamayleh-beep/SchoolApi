using System.ComponentModel.DataAnnotations;

namespace SchoolApi.Models;

public class Classroom : BaseEntity
{

 
    public string Name { get; set; } = "";

    public int Year { get; set; }

    // One-to-Many: one classroom has many students
    public List<Student> Students { get; set; } = new();
}