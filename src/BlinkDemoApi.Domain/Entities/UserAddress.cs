namespace BlinkDemoApi.Domain.Entities;

/// <summary>
/// Saved delivery addresses for a user.
/// </summary>
public class UserAddress : BaseEntity
{
    /// <summary>
    /// FK to users.id (UUID).
    /// </summary>
    public Guid UserId { get; set; }

    public string Label { get; set; } = string.Empty; // e.g. Home, Office
    public string AddressLine { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    public decimal? Lat { get; set; }
    public decimal? Lng { get; set; }

    public bool IsDefault { get; set; }

    public User? User { get; set; }
}
