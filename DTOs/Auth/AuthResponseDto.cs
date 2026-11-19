namespace SchoolApi.DTOs.Auth;

// What the API returns after a successful login
public class AuthResponseDto
{
    public string Token { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
}
