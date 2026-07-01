using BlinkDemoApi.Domain.Entities;

namespace BlinkDemoApi.Application.Auth.Interfaces;

/// <summary>
/// Token generator abstraction.
/// Api layer depends on Application abstractions, Infrastructure provides implementation.
/// </summary>
public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) CreateAccessToken(User user);
    (string RefreshToken, string RefreshTokenHash, DateTime ExpiresAt) CreateRefreshToken();
    string HashRefreshToken(string refreshToken);
}
