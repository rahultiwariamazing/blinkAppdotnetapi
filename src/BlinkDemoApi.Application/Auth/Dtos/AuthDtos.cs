using System;

namespace BlinkDemoApi.Application.Auth.Dtos;

/// <summary>
/// Signup request.
/// Password comes from the client; server converts to PasswordHash.
/// </summary>
public sealed record RegisterRequest(string Name, string Mobile, string Email, string Password);

/// <summary>
/// Login request using mobile + password (as you requested).
/// </summary>
public sealed record LoginRequest(string Mobile, string Password);

/// <summary>
/// Refresh request.
/// AccessToken is optional; refresh token is mandatory.
/// </summary>
public sealed record RefreshRequest(string RefreshToken);

/// <summary>
/// Standard auth response.
/// </summary>
public sealed record AuthResponse(
    Guid UserId,
    string Name,
    string Mobile,
    string Email,
    string AccessToken,
    DateTime AccessTokenExpiresAt,
    string RefreshToken,
    DateTime RefreshTokenExpiresAt
);
