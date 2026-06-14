using BlinkDemoApi.Domain.Entities;

namespace BlinkDemoApi.Application.Auth.Interfaces;

public interface IAuthRepository
{
    Task<bool> MobileExistsAsync(string mobile, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    Task CreateUserAsync(User user, CancellationToken ct);

    Task<User?> GetUserByMobileAsync(string mobile, CancellationToken ct);
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct);

    Task UpsertRefreshTokenAsync(Guid userId, string refreshTokenHash, DateTime expiresAt, string? deviceInfo, CancellationToken ct);
    Task<RefreshToken?> GetActiveRefreshTokenAsync(Guid userId, string refreshTokenHash, CancellationToken ct);
    Task RevokeRefreshTokenAsync(Guid refreshTokenId, CancellationToken ct);
}