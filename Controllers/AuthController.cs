using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.Auth;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Auth;
using SchoolApi.Extensions;
using SchoolApi.Services.Abstractions;

namespace SchoolApi.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST api/auth/login   (anyone can try)
    [HttpPost("login")]
    [ProducesResponseType(typeof(BaseResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        return ToActionResult(await _authService.LoginAsync(dto));
    }

    // POST api/auth/register   (in a school, only the admin creates accounts)
    [Authorize(Roles = Roles.Admin)]
    [HttpPost("register")]
    [ProducesResponseType(typeof(BaseResponse<UserDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        return ToActionResult(await _authService.RegisterAsync(dto));
    }

    // GET api/auth/me   (who am I? read from the token)
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(BaseResponse<CurrentUser>), StatusCodes.Status200OK)]
    public IActionResult Me()
    {
        return ToActionResult(BaseResponse<CurrentUser>.Success(User.ToCurrentUser()));
    }
}
