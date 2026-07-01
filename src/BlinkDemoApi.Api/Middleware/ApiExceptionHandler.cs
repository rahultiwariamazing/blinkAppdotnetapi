using BlinkDemoApi.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace BlinkDemoApi.Api.Middleware;

/// <summary>
/// Global exception handler.
/// - Converts exceptions into consistent JSON.
/// - Adds traceId so logs can be correlated with a client-reported issue.
/// </summary>
public sealed class ApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        // FluentValidation errors
        if (exception is ValidationException ve)
        {
            var errors = ve.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var payload = ApiResponse<object>.Fail(
                message: "Validation failed.",
                statusCode: httpContext.Response.StatusCode,
                traceId: traceId,
                errorCode: ErrorCodes.ValidationFailed
            );

            // Put field-level errors into Data.
            payload.Data = new { errors };

            await httpContext.Response.WriteAsJsonAsync(payload, cancellationToken);
            return true;
        }

        // Unknown errors
        _logger.LogError(exception, "Unhandled exception. TraceId={TraceId}", traceId);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.Fail("Something went wrong.", httpContext.Response.StatusCode, traceId, ErrorCodes.ServerError),
            cancellationToken
        );
        return true;
    }
}
