namespace SchoolApi.Auth;

// The role names, written once, so we never mistype "Teacher" as "teacher"
public static class Roles
{
    public const string Admin = "Admin";
    public const string Teacher = "Teacher";
    public const string Student = "Student";

    // For [Authorize(Roles = ...)]: a comma means "any of these"
    public const string AdminOrTeacher = Admin + "," + Teacher;
}
