using BlinkDemoApi.Application.Orders.Dtos;
using BlinkDemoApi.Application.Orders.Services;
using BlinkDemoApi.Domain.Entities;
using BlinkDemoApi.Domain.Enums;
using BlinkDemoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlinkDemoApi.Infrastructure.Repositories;

/// <summary>
/// Repository for handling all order-related DB operations.
/// Updated to support AddressId linking.
/// </summary>
public sealed class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;

    public OrderRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Places a new order:
    /// - Validates cart
    /// - Validates stock
    /// - Validates address (if provided)
    /// - Creates Order + OrderItems
    /// - Links AddressId to order
    /// - Decreases stock
    /// - Clears cart
    /// </summary>
    public async Task<(bool Ok, string? Error, OrderDto? Order)> PlaceAsync(
        Guid userId,
        PlaceOrderRequest req,
        CancellationToken ct)
    {
        // ------------------------------
        // 1) Validate cart
        // ------------------------------
        var cart = await _db.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .ToListAsync(ct);

        if (cart.Count == 0)
            return (false, "Cart is empty.", null);

        foreach (var c in cart)
        {
            if (c.Product is null)
                return (false, "Cart contains invalid product.", null);

            if (c.Quantity > c.Product.StockQty)
                return (false, $"Not enough stock for {c.Product.Name}.", null);
        }

        // ------------------------------
        // 2) Validate user address (if provided)
        // ------------------------------
        if (req.AddressId.HasValue)
        {
            var addressExists = await _db.UserAddresses
                .AnyAsync(a => a.Id == req.AddressId && a.UserId == userId, ct);

            if (!addressExists)
                return (false, "Invalid or unauthorized address.", null);
        }

        // ------------------------------
        // 3) Begin DB transaction
        // ------------------------------
        using var tx = await _db.Database.BeginTransactionAsync(ct);

        // ------------------------------
        // 4) Create order master record
        // ------------------------------
        var order = new Order
        {
            UserId = userId,
            AddressId = req.AddressId,            // <--- NEW: save linked addressId
            Status = OrderStatus.PLACED
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(ct);

        // ------------------------------
        // 5) Create order items + calculate total
        // ------------------------------
        decimal total = 0;

        foreach (var c in cart)
        {
            var product = c.Product!;
            var price = product.Price;
            var subtotal = price * c.Quantity;
            total += subtotal;

            // Create order item
            _db.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = c.Quantity,
                PriceAtPurchase = price,
                Subtotal = subtotal
            });

            // Reduce stock
            product.StockQty -= c.Quantity;
        }

        // Save total amount
        order.TotalAmount = total;

        // ------------------------------
        // 6) Clear cart
        // ------------------------------
        _db.CartItems.RemoveRange(cart);

        // Save all changes
        await _db.SaveChangesAsync(ct);

        // Commit transaction
        await tx.CommitAsync(ct);

        // ------------------------------
        // 7) Return the created order DTO
        // ------------------------------
        var dto = await GetMyOrderDetailAsync(userId, order.Id, ct);
        return (true, null, dto);
    }

    /// <summary>
    /// Gets latest 50 orders for a user.
    /// Includes order items + product names.
    /// </summary>
    public async Task<IReadOnlyList<OrderDto>> GetMyOrdersAsync(Guid userId, CancellationToken ct)
    {
        return await _db.Orders.AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Take(50)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Select(o => new OrderDto(
                o.Id,
                o.TotalAmount,
                o.Status,
                o.CreatedAt,
                o.Items.OrderBy(i => i.CreatedAt).Select(i => new OrderItemDto(
                    i.Id,
                    i.ProductId,
                    i.Product!.Name,
                    i.Quantity,
                    i.PriceAtPurchase,
                    i.Subtotal
                )).ToList()
            ))
            .ToListAsync(ct);
    }

    /// <summary>
    /// Loads a specific order with its items.
    /// </summary>
    public async Task<OrderDto?> GetMyOrderDetailAsync(Guid userId, Guid orderId, CancellationToken ct)
    {
        return await _db.Orders.AsNoTracking()
            .Where(o => o.UserId == userId && o.Id == orderId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Select(o => new OrderDto(
                o.Id,
                o.TotalAmount,
                o.Status,
                o.CreatedAt,
                o.Items.OrderBy(i => i.CreatedAt).Select(i => new OrderItemDto(
                    i.Id,
                    i.ProductId,
                    i.Product!.Name,
                    i.Quantity,
                    i.PriceAtPurchase,
                    i.Subtotal
                )).ToList()
            ))
            .FirstOrDefaultAsync(ct);
    }

    /// <summary>
    /// Cancels an order if not delivered.
    /// </summary>
    public async Task<(bool Ok, string? Error)> CancelAsync(Guid userId, Guid orderId, CancellationToken ct)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.UserId == userId && o.Id == orderId, ct);

        if (order is null)
            return (false, "Order not found.");

        if (order.Status == OrderStatus.DELIVERED)
            return (false, "Delivered orders cannot be cancelled.");

        if (order.Status == OrderStatus.CANCELLED)
            return (true, null);

        order.Status = OrderStatus.CANCELLED;
        await _db.SaveChangesAsync(ct);

        return (true, null);
    }
}