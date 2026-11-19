using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Auth;

namespace SchoolApi.Services.Abstractions;

public interface IAuthService
{
    Task<BaseResponse<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<BaseResponse<UserDto>> RegisterAsync(RegisterDto dto);
}
