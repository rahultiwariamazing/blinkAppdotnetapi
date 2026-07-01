using BlinkDemoApi.Application.Cart.Dtos;

namespace BlinkDemoApi.Application.Cart.Services;

/// <summary>
/// Cart use-cases. Quantity=0 removes item.
/// </summary>
public sealed class CartService
{
    private readonly ICartRepository _repo;

    public CartService(ICartRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<CartItemDto>> GetMyCartAsync(Guid userId, CancellationToken ct)
        => _repo.GetCartAsync(userId, ct);

    public Task<(bool Ok, string? Error, IReadOnlyList<CartItemDto> Cart)> UpsertAsync(Guid userId, UpsertCartItemRequest req, CancellationToken ct)
        => _repo.UpsertAsync(userId, req, ct);

    public Task<bool> RemoveAsync(Guid userId, Guid productId, CancellationToken ct)
        => _repo.RemoveAsync(userId, productId, ct);

    public Task ClearAsync(Guid userId, CancellationToken ct)
        => _repo.ClearAsync(userId, ct);
}

public interface ICartRepository
{
    Task<IReadOnlyList<CartItemDto>> GetCartAsync(Guid userId, CancellationToken ct);
    Task<(bool Ok, string? Error, IReadOnlyList<CartItemDto> Cart)> UpsertAsync(Guid userId, UpsertCartItemRequest req, CancellationToken ct);
    Task<bool> RemoveAsync(Guid userId, Guid productId, CancellationToken ct);
    Task ClearAsync(Guid userId, CancellationToken ct);
}
