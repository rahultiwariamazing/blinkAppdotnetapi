using BlinkDemoApi.Application.Orders.Dtos;

namespace BlinkDemoApi.Application.Orders.Services;

/// <summary>
/// Order use-cases: place, list, detail, cancel.
/// </summary>
public sealed class OrderService
{
    private readonly IOrderRepository _repo;

    public OrderService(IOrderRepository repo)
    {
        _repo = repo;
    }

    public Task<(bool Ok, string? Error, OrderDto? Order)> PlaceAsync(Guid userId, PlaceOrderRequest req, CancellationToken ct)
        => _repo.PlaceAsync(userId, req, ct);

    public Task<IReadOnlyList<OrderDto>> GetMyOrdersAsync(Guid userId, CancellationToken ct)
        => _repo.GetMyOrdersAsync(userId, ct);

    public Task<OrderDto?> GetMyOrderDetailAsync(Guid userId, Guid orderId, CancellationToken ct)
        => _repo.GetMyOrderDetailAsync(userId, orderId, ct);

    public Task<(bool Ok, string? Error)> CancelAsync(Guid userId, Guid orderId, CancellationToken ct)
        => _repo.CancelAsync(userId, orderId, ct);
}

public interface IOrderRepository
{
    Task<(bool Ok, string? Error, OrderDto? Order)> PlaceAsync(Guid userId, PlaceOrderRequest req, CancellationToken ct);
    Task<IReadOnlyList<OrderDto>> GetMyOrdersAsync(Guid userId, CancellationToken ct);
    Task<OrderDto?> GetMyOrderDetailAsync(Guid userId, Guid orderId, CancellationToken ct);
    Task<(bool Ok, string? Error)> CancelAsync(Guid userId, Guid orderId, CancellationToken ct);
}