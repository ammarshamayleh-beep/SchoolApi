using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SchoolApi.Models;


public class User : BaseEntity
{
  
    public string Email { get; set; } = "";

    // We save the HASH of the password, never the password itself
   
    public string PasswordHash { get; set; } = "";

    // Admin, Teacher or Student (see Auth/Roles.cs)
  
    public string Role { get; set; } = "";

    // A teacher account points to its teacher (null for the others)
    public Guid? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    // A student account points to its student (null for the others)
    public Guid? StudentId { get; set; }
    public Student? Student { get; set; }
}
