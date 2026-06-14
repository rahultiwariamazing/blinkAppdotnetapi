using BlinkDemoApi.Application.Addresses.Dtos;

namespace BlinkDemoApi.Application.Addresses.Services;

/// <summary>
/// Address CRUD use-cases.
/// </summary>
public sealed class AddressService
{
    private readonly IAddressRepository _repo;

    public AddressService(IAddressRepository repo)
    {
        ArgumentNullException.ThrowIfNull(repo);
        _repo = repo;
    }

    public Task<IReadOnlyList<AddressDto>> GetMyAddressesAsync(Guid userId, CancellationToken ct)
        => _repo.GetByUserAsync(userId, ct);

    public Task<AddressDto> CreateAsync(Guid userId, CreateAddressRequest req, CancellationToken ct)
        => _repo.CreateAsync(userId, req, ct);

    public Task<AddressDto?> UpdateAsync(Guid userId, Guid addressId, UpdateAddressRequest req, CancellationToken ct)
        => _repo.UpdateAsync(userId, addressId, req, ct);

    public Task<bool> DeleteAsync(Guid userId, Guid addressId, CancellationToken ct)
        => _repo.DeleteAsync(userId, addressId, ct);

    public Task<bool> SetDefaultAsync(Guid userId, Guid addressId, CancellationToken ct)
        => _repo.SetDefaultAsync(userId, addressId, ct);
}

public interface IAddressRepository
{
    Task<IReadOnlyList<AddressDto>> GetByUserAsync(Guid userId, CancellationToken ct);
    Task<AddressDto> CreateAsync(Guid userId, CreateAddressRequest req, CancellationToken ct);
    Task<AddressDto?> UpdateAsync(Guid userId, Guid addressId, UpdateAddressRequest req, CancellationToken ct);
    Task<bool> DeleteAsync(Guid userId, Guid addressId, CancellationToken ct);
    Task<bool> SetDefaultAsync(Guid userId, Guid addressId, CancellationToken ct);
}
