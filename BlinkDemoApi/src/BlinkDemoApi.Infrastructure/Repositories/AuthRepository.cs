using BlinkDemoApi.Application.Auth.Interfaces;
using BlinkDemoApi.Application.Auth.Services;
using BlinkDemoApi.Domain.Entities;
using BlinkDemoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlinkDemoApi.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of auth repository.
/// </summary>
public sealed class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _db;

    public AuthRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<bool> MobileExistsAsync(string mobile, CancellationToken ct)
        => _db.Users.AnyAsync(u => u.Mobile == mobile, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct)
        => _db.Users.AnyAsync(u => u.Email == email.ToLower(), ct);

    public async Task CreateUserAsync(User user, CancellationToken ct)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }

    public Task<User?> GetUserByMobileAsync(string mobile, CancellationToken ct)
        => _db.Users.FirstOrDefaultAsync(u => u.Mobile == mobile, ct);

    public Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct)
        => _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

    public async Task UpsertRefreshTokenAsync(
        Guid userId,
        string refreshTokenHash,
        DateTime expiresAt,
        string? deviceInfo,
        CancellationToken ct)
    {
        // Simple strategy: insert new refresh token row.
        // You can later keep multiple tokens per user/device.
        var row = new RefreshToken
        {
            UserId = userId,
            TokenHash = refreshTokenHash,
            ExpiresAt = expiresAt,
            DeviceInfo = deviceInfo
        };

        _db.RefreshTokens.Add(row);
        await _db.SaveChangesAsync(ct);
    }

    public Task<RefreshToken?> GetActiveRefreshTokenAsync(Guid userId, string refreshTokenHash, CancellationToken ct)
        => _db.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                x.TokenHash == refreshTokenHash &&
                x.RevokedAt == null &&
                x.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public async Task RevokeRefreshTokenAsync(Guid refreshTokenId, CancellationToken ct)
    {
        var row = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Id == refreshTokenId, ct);
        if (row is null) return;

        row.RevokedAt = DateTime.UtcNow;
        row.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }
}
