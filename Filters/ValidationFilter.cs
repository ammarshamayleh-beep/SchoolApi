using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchoolApi.DTOs.Common;
using SchoolApi.DTOs.Classrooms;

namespace SchoolApi.Filters;

// Runs before every action. If an argument has a validator, it runs it.
// Any broken rule stops the request with 400 Bad Request.
public class ValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // The action's arguments, for example: the DTO from the body, or the query from the URL
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null)
            {
                continue;
            }

            // Is there a validator for this type? For example IValidator<ClassroomRequestDto>
            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            var validator = _serviceProvider.GetService(validatorType) as IValidator;

            if (validator == null)
            {
                continue;   // no validator, nothing to check (for example a Guid id)
            }

            var result = await validator.ValidateAsync(new ValidationContext<object>(argument));

            if (!result.IsValid)
            {
                string error = string.Join(" ", result.Errors.Select(e => e.ErrorMessage));

                context.Result = new BadRequestObjectResult(
                    BaseResponse<object>.Failure(StatusCodes.Status400BadRequest, error));
                return;   // stop here, the action never runs
            }
        }

        await next();   // everything is valid, run the action
    }
}
