using FluentValidation;
using MediaWatchlistApi.Dtos;

namespace MediaWatchlistApi.Application;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if (validator is not null)
        {
            var entity = context.Arguments.OfType<T>().FirstOrDefault();
            if (entity is not null)
            {
                var validationResult = await validator.ValidateAsync(entity);
                if (!validationResult.IsValid)
                {
                    return Results.BadRequest(ApiResponseError.Fail(
                        "Bad Request",
                        validationResult.Errors
                            .Select(error => new ApiError(error.ErrorCode, error.ErrorMessage))
                            .ToList(),
                        context.HttpContext.TraceIdentifier
                    ));
                }
            }
        }

        return await next(context);
    }
}