namespace BlinkDemoApi.Application.Cart.Dtos;

public sealed record UpsertCartItemRequest(Guid ProductId, int Quantity);

public sealed record CartItemDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string? ImageUrl,
    decimal Price,
    int Quantity,
    string Unit
);
