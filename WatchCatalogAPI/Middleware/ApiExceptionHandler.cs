using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WatchCatalogAPI.Services;

namespace WatchCatalogAPI.Middleware;

public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ProductNumberConflictException)
        {
            return false;
        }

        logger.LogWarning(exception, "A duplicate watch product number was submitted.");
        httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Product number already exists",
            Detail = "Choose a unique product number.",
            Instance = httpContext.Request.Path
        }, cancellationToken);
        return true;
    }
}
