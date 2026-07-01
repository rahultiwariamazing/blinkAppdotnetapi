using BlinkDemoApi.Application.Me.Dtos;
using BlinkDemoApi.Application.Me.Services;
using BlinkDemoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlinkDemoApi.Infrastructure.Repositories;

public sealed class MeRepository : IMeRepository
{
    private readonly AppDbContext _db;

    public MeRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<BootstrapResponse?> GetBootstrapAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);

        if (user is null) return null;

        var addresses = await _db.UserAddresses.AsNoTracking()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.CreatedAt)
            .Select(a => new AddressDto(
                a.Id,
                a.Label,
                a.AddressLine,
                a.Pincode,
                a.City,
                a.Lat,
                a.Lng,
                a.IsDefault))
            .ToListAsync(ct);

        var cart = await _db.CartItems.AsNoTracking()
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .OrderByDescending(c => c.CreatedAt)
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

        var recentOrders = await _db.Orders.AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Select(o => new OrderDto(
                o.Id,
                o.TotalAmount,
                o.Status,
                o.CreatedAt,
                o.Items
                    .OrderBy(i => i.CreatedAt)
                    .Select(i => new OrderItemDto(
                        i.Id,
                        i.ProductId,
                        i.Product!.Name,
                        i.Quantity,
                        i.PriceAtPurchase,
                        i.Subtotal
                    ))
                    .ToList()
            ))
            .ToListAsync(ct);

        return new BootstrapResponse(
            new UserSummaryDto(user.Id, user.Name, user.Mobile, user.Email),
            addresses,
            cart,
            recentOrders
        );
    }
}