namespace SchoolApi.Auth;

// Who is calling the API, read from the token
public class CurrentUser
{
    public Guid? UserId { get; init; }
    public string Role { get; init; } = "";
    public Guid? TeacherId { get; init; }
    public Guid? StudentId { get; init; }

    public bool IsAdmin => Role == Roles.Admin;
    public bool IsTeacher => Role == Roles.Teacher;
    public bool IsStudent => Role == Roles.Student;
}
