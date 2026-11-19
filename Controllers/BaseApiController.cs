using Microsoft.AspNetCore.Mvc;
using SchoolApi.DTOs.Common;

namespace SchoolApi.Controllers;

// The parent of every controller. It turns a BaseResponse into an HTTP response.
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    // The HTTP status code comes from the response itself (200, 201, 404, 409...)
    protected IActionResult ToActionResult<T>(BaseResponse<T> response)
    {
        return StatusCode(response.StatusCode, response);
    }
}
