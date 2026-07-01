namespace BlinkDemoApi.Domain.Entities;

/// <summary>
/// Snapshot of items purchased in an order.
/// PriceAtPurchase is stored so invoices remain correct even after price changes.
/// </summary>
public class OrderItem : BaseEntity
{
    /// <summary>
    /// FK to orders.id (UUID).
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// FK to products.id (UUID).
    /// </summary>
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }
    public decimal Subtotal { get; set; }

    public Order? Order { get; set; }
    public Product? Product { get; set; }
}
