using System.Security.Claims;
using BlinkDemoApi.Application.Auth.Dtos;
using BlinkDemoApi.Application.Auth.Services;
using BlinkDemoApi.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlinkDemoApi.Api.Controllers;

/// <summary>
/// Authentication endpoints.
/// Note: Login/Register do NOT require bearer token.
/// </summary>
[Route("api/auth")]
public sealed class AuthController : BaseApiController
{
    private readonly AuthService _auth;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthController(AuthService auth, IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator)
    {
        _auth = auth;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        await _registerValidator.ValidateAndThrowAsync(request, ct);

        var deviceInfo = Request.Headers.UserAgent.ToString();
        var (resp, errorCode, message) = await _auth.RegisterAsync(request, deviceInfo, ct);
        if (resp is null)
        {
            var status = errorCode is ErrorCodes.DuplicateMobile or ErrorCodes.DuplicateEmail
                ? StatusCodes.Status409Conflict
                : StatusCodes.Status400BadRequest;

            return Fail(message, status, errorCode);
        }

        return OkResponse(resp, message);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        await _loginValidator.ValidateAndThrowAsync(request, ct);

        var deviceInfo = Request.Headers.UserAgent.ToString();
        var (resp, errorCode, message) = await _auth.LoginAsync(request, deviceInfo, ct);
        if (resp is null)
            return Fail(message, StatusCodes.Status401Unauthorized, errorCode);

        return OkResponse(resp, message);
    }

    [HttpPost("refresh")]
    [Authorize]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
    {
        // UserId comes from the access token ("sub").
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(sub, out var userId))
            return Fail("Invalid token.", StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized);

        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return Fail("RefreshToken is required.", StatusCodes.Status400BadRequest, ErrorCodes.ValidationFailed);

        var deviceInfo = Request.Headers.UserAgent.ToString();
        var (resp, errorCode, message) = await _auth.RefreshAsync(userId, request.RefreshToken, deviceInfo, ct);
        if (resp is null)
            return Fail(message, StatusCodes.Status401Unauthorized, errorCode);

        return OkResponse(resp, message);
    }
}
