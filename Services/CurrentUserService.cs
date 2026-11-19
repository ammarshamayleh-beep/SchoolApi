using SchoolApi.Extensions;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
            {
                return Guid.Empty;
            }

            return user.ToCurrentUser().UserId ?? Guid.Empty;
        }
    }
}
