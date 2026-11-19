using System.Security.Claims;

namespace SchoolApi.Auth;

// Our own claim names (the standard ones come from ClaimTypes)
public static class AppClaims
{
    public const string TeacherId = "teacherId";
    public const string StudentId = "studentId";
}
