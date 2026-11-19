using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SchoolApi.Models;


public class Subject : BaseEntity
{
 
    public string Name { get; set; } = "";

   
    public string Code { get; set; } = "";

    // Optional: a subject can exist without a teacher
    public Guid? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
    // Many-to-Many with students, through grades
    public List<Grade> Grades { get; set; } = new();
}