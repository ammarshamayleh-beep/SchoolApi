using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SchoolApi.Models;


public class Grade : BaseEntity
{
    // Link to the student
    public Guid StudentId { get; set; } 
    public Student? Student { get; set; }

    // Link to the subject
    public Guid SubjectId { get; set; }
    public Subject? Subject { get; set; }

    // Extra data about this grade

    public string Term { get; set; } = "";

   
    public decimal Score { get; set; }
}