namespace SchoolApi.DTOs.Auth;

// An account, without the password hash
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public Guid? TeacherId { get; set; }
    public Guid? StudentId { get; set; }
}
