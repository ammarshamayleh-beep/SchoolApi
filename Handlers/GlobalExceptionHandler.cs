using Microsoft.AspNetCore.Diagnostics;
using SchoolApi.DTOs.Common;

namespace SchoolApi.Handlers;

// Catches any exception that nobody caught, and returns a 500 BaseResponse.
// The details stay in the log; the client only gets a short message.
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = BaseResponse<object>.Failure(
            StatusCodes.Status500InternalServerError,
            "Something went wrong. Please try again later.");

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;   // handled: nothing else runs for this exception
    }
}
