using System.Security.Claims;
using SchoolApi.Auth;

namespace SchoolApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    // In a controller: var me = User.ToCurrentUser();
    public static CurrentUser ToCurrentUser(this ClaimsPrincipal user)
    {
        return new CurrentUser
        {
            UserId = ToGuid(user.FindFirstValue(ClaimTypes.NameIdentifier)) ?? Guid.Empty,
            Role = user.FindFirstValue(ClaimTypes.Role) ?? "",
            TeacherId = ToGuid(user.FindFirstValue(AppClaims.TeacherId)),
            StudentId = ToGuid(user.FindFirstValue(AppClaims.StudentId))
        };
    }

    private static Guid? ToGuid(string? value)
    {
        return Guid.TryParse(value, out Guid number) ? number : null;
    }
}
