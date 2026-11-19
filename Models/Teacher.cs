using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SchoolApi.Models;


public class Teacher : BaseEntity
{
    public string FirstName { get; set; } = "";

   
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";

    public DateTime HireDate { get; set; }

    // One-to-Many: one teacher teaches many subjects
    public List<Subject> Subjects { get; set; } = new();
}
