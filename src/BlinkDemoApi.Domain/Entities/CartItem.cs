namespace BlinkDemoApi.Domain.Entities;

/// <summary>
/// User cart item (one product + quantity).
/// </summary>
public class CartItem : BaseEntity
{
    /// <summary>
    /// FK to users.id (UUID).
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// FK to products.id (UUID).
    /// </summary>
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public User? User { get; set; }
    public Product? Product { get; set; }
}
