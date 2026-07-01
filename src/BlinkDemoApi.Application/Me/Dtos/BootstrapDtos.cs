using BlinkDemoApi.Domain.Enums;

namespace BlinkDemoApi.Application.Me.Dtos;

public sealed record UserSummaryDto(Guid Id, string Name, string Mobile, string Email);

public sealed record AddressDto(
    Guid Id,
    string Label,
    string AddressLine,
    string Pincode,
    string City,
    decimal? Lat,
    decimal? Lng,
    bool IsDefault
);

public sealed record CartItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string? ImageUrl,
    decimal Price,
    int Quantity,
    string Unit
);

public sealed record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal PriceAtPurchase,
    decimal Subtotal
);

public sealed record OrderDto(
    Guid Id,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime CreatedAt,
    IReadOnlyList<OrderItemDto> Items
);

public sealed record BootstrapResponse(
    UserSummaryDto User,
    IReadOnlyList<AddressDto> Addresses,
    IReadOnlyList<CartItemDto> Cart,
    IReadOnlyList<OrderDto> RecentOrders
);
