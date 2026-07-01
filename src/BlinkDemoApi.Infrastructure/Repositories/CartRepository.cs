using BlinkDemoApi.Application.Cart.Dtos;
using BlinkDemoApi.Application.Cart.Services;
using BlinkDemoApi.Domain.Entities;
using BlinkDemoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlinkDemoApi.Infrastructure.Repositories;

public sealed class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;

    public CartRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CartItemDto>> GetCartAsync(Guid userId, CancellationToken ct)
    {
        return await _db.CartItems.AsNoTracking()
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .OrderByDescending(c => c.Id)
            .Select(c => new CartItemDto(
                c.Id,
                c.ProductId,
                c.Product!.Name,
                c.Product.ImageUrl,
                c.Product.Price,
                c.Quantity,
                c.Product.Unit
            ))
            .ToListAsync(ct);
    }

    public async Task<(bool Ok, string? Error, IReadOnlyList<CartItemDto> Cart)> UpsertAsync(
        Guid userId,
        UpsertCartItemRequest req,
        CancellationToken ct)
    {
        if (req.ProductId == Guid.Empty)
            return (false, "Invalid productId.", await GetCartAsync(userId, ct));

        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == req.ProductId, ct);
        if (product is null)
            return (false, "Product not found.", await GetCartAsync(userId, ct));

        var row = await _db.CartItems
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == req.ProductId, ct);

        // If no row → create with initial qty
        if (row is null)
        {
            var startQty = req.Quantity;
            if (startQty <= 0)
                return (false, "Quantity must be positive.", await GetCartAsync(userId, ct));

            if (startQty > product.StockQty)
                return (false, "Not enough stock.", await GetCartAsync(userId, ct));

            row = new CartItem
            {
                UserId = userId,
                ProductId = req.ProductId,
                Quantity = startQty
            };

            _db.CartItems.Add(row);
            await _db.SaveChangesAsync(ct);
            return (true, null, await GetCartAsync(userId, ct));
        }

        // Existing row → apply increment/decrement
        var newQty = row.Quantity + req.Quantity;

        // If new quantity <= 0 → remove
        if (newQty <= 0)
        {
            _db.CartItems.Remove(row);
            await _db.SaveChangesAsync(ct);
            return (true, null, await GetCartAsync(userId, ct));
        }

        // Stock check
        if (newQty > product.StockQty)
            return (false, "Not enough stock.", await GetCartAsync(userId, ct));

        row.Quantity = newQty;

        await _db.SaveChangesAsync(ct);
        return (true, null, await GetCartAsync(userId, ct));
    }

    public async Task<bool> RemoveAsync(Guid userId, Guid productId, CancellationToken ct)
    {
        var row = await _db.CartItems
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId, ct);

        if (row is null) return false;

        _db.CartItems.Remove(row);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task ClearAsync(Guid userId, CancellationToken ct)
    {
        var rows = await _db.CartItems.Where(x => x.UserId == userId).ToListAsync(ct);
        if (rows.Count == 0) return;

        _db.CartItems.RemoveRange(rows);
        await _db.SaveChangesAsync(ct);
    }
}