using BlinkDemoApi.Domain.Enums;

namespace BlinkDemoApi.Application.Orders.Dtos;

/// <summary>
/// Request payload for placing an order.
/// For this demo we only LINK to an existing user address via AddressId (no snapshot).
/// If null, order will be created without address linkage.
/// </summary>
public sealed record PlaceOrderRequest(Guid? AddressId);

/// <summary>
/// Item line returned with an order.
/// This is a snapshot of the purchased product state (price at purchase, subtotal).
/// </summary>
public sealed record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal PriceAtPurchase,
    decimal Subtotal
);

/// <summary>
/// Order summary/detail returned by APIs.
/// NOTE: We are NOT exposing AddressId here to keep the mobile UI unchanged.
/// If needed later, we can extend this DTO without breaking existing flows.
/// </summary>
public sealed record OrderDto(
    Guid Id,
    decimal TotalAmount,
    OrderStatus Status,
    DateTime CreatedAt,
    IReadOnlyList<OrderItemDto> Items
);
