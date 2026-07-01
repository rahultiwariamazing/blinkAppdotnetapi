using BlinkDemoApi.Domain.Enums;

namespace BlinkDemoApi.Domain.Entities;

/// <summary>
/// Represents an app user.
/// We store PasswordHash only (never store plain passwords).
/// Mobile + Email are unique.
/// </summary>
public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Mobile { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// BCrypt hash of the password.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    // Navigation properties (EF Core will populate these when you Include())
    public List<UserAddress> Addresses { get; set; } = new();
    public List<CartItem> CartItems { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
