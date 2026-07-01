namespace BlinkDemoApi.Domain.Entities;

/// <summary>
/// Stores refresh tokens for JWT auth.
/// We store ONLY a hash of the refresh token for security.
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// FK to users.id (UUID).
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Hash of the refresh token string.
    /// Never store the raw token in DB.
    /// </summary>
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Optional: device info helps with security auditing.
    /// </summary>
    public string? DeviceInfo { get; set; }

    public User? User { get; set; }

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}