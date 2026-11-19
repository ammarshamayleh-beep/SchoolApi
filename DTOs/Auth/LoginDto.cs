namespace SchoolApi.DTOs.Auth;

// What the client sends to log in
public class LoginDto
{
  
    public string Email { get; set; } = "";

    public string Password { get; set; } = "";
}
