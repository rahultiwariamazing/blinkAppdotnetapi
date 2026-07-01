namespace BlinkDemoApi.Domain.Entities;

/// <summary>
/// Base entity containing common audit fields.
/// IMPORTANT: Postgres tables are using UUID primary keys (id UUID),
/// so we use Guid here to match the database schema.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Primary key (UUID).
    /// Matches Postgres: id UUID DEFAULT gen_random_uuid()
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// When this row was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this row was last updated (UTC).
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}