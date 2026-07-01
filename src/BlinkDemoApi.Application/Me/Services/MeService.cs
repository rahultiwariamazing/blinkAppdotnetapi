using BlinkDemoApi.Application.Me.Dtos;

namespace BlinkDemoApi.Application.Me.Services;

/// <summary>
/// Provides the /me/bootstrap response for the app startup.
/// </summary>
public sealed class MeService
{
    private readonly IMeRepository _repo;

    public MeService(IMeRepository repo)
    {
        _repo = repo;
    }

    public Task<BootstrapResponse?> GetBootstrapAsync(Guid userId, CancellationToken ct)
        => _repo.GetBootstrapAsync(userId, ct);
}

/// <summary>
/// Repository abstraction for data needed by /me/bootstrap.
/// </summary>
public interface IMeRepository
{
    Task<BootstrapResponse?> GetBootstrapAsync(Guid userId, CancellationToken ct);
}