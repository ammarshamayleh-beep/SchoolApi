namespace SchoolApi.DTOs.Auth;

// What the admin sends to create an account
public class RegisterDto
{
        public string Role { get; set; } = "";
        public string Email { get; set; } = "";

         public string Password { get; set; } = "";
    

    public Guid? TeacherId { get; set; }   // needed when Role is Teacher
    public Guid? StudentId { get; set; }   // needed when Role is Student
}
