using System.Security.Claims;
using BlinkDemoApi.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace BlinkDemoApi.Api.Controllers;

public abstract class BaseApiController : ControllerBase
{
    protected string TraceId => HttpContext.TraceIdentifier;

    // Parse GUID user id from JWT "sub" (or NameIdentifier fallback).
    protected Guid? CurrentUserId
    {
        get
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            return Guid.TryParse(sub, out var userId) ? userId : null;
        }
    }

    protected bool TryGetCurrentUserId(out Guid userId)
    {
        if (CurrentUserId is { } id)
        {
            userId = id;
            return true;
        }

        userId = Guid.Empty;
        return false;
    }

    protected IActionResult InvalidToken()
        => Fail("Invalid token.", StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized);

    protected IActionResult OkResponse<T>(T data, string message = "Success", int statusCode = StatusCodes.Status200OK)
        => StatusCode(statusCode, ApiResponse<T>.Ok(data, message, statusCode, TraceId));

    protected IActionResult Fail(string message, int statusCode, string? errorCode = null)
        => StatusCode(statusCode, ApiResponse<object>.Fail(message, statusCode, TraceId, errorCode));
}
