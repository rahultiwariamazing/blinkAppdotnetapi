using BlinkDemoApi.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlinkDemoApi.Domain.Entities;

/// <summary>
/// Order master.
/// TotalAmount is stored to keep order totals stable even if product prices change later.
/// Now includes AddressId to link with user_addresses table.
/// </summary>
public class Order : BaseEntity
{
    /// <summary>
    /// FK to users.id (UUID)
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// FK to user_addresses.id (UUID)
    /// This links the selected delivery address of the user.
    /// NOTE: We are not storing snapshot address fields,
    /// only linking to the existing address for this demo.
    /// </summary>
    [Column("addressid")]
    public Guid? AddressId { get; set; }

    /// <summary>
    /// Final order amount (static once order is placed)
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Status of the order (PLACED, CONFIRMED, DELIVERED, CANCELLED)
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.PLACED;

    /// <summary>
    /// Navigation references
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Order items purchased under this order
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();
}