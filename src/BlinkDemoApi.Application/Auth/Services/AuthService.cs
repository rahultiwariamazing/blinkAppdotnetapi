using BlinkDemoApi.Application.Auth.Dtos;
using BlinkDemoApi.Application.Auth.Interfaces;
using BlinkDemoApi.Application.Common;
using BlinkDemoApi.Domain.Entities;

namespace BlinkDemoApi.Application.Auth.Services;

/// <summary>
/// Auth use-cases: register, login, refresh.
/// </summary>
public sealed class AuthService
{
    private readonly IAuthRepository _repo;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _tokens;

    public AuthService(IAuthRepository repo, IPasswordHasher hasher, IJwtTokenService tokens)
    {
        _repo = repo;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<(AuthResponse? Response, string? ErrorCode, string Message)>
        RegisterAsync(RegisterRequest request, string? deviceInfo, CancellationToken ct)
    {
        if (await _repo.MobileExistsAsync(request.Mobile, ct))
            return (null, ErrorCodes.DuplicateMobile, "Mobile number already registered.");

        if (await _repo.EmailExistsAsync(request.Email, ct))
            return (null, ErrorCodes.DuplicateEmail, "Email already registered.");

        var user = new User
        {
            Name = request.Name.Trim(),
            Mobile = request.Mobile.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = _hasher.Hash(request.Password)
        };

        await _repo.CreateUserAsync(user, ct);

        var (access, accessExp) = _tokens.CreateAccessToken(user);
        var (refresh, refreshHash, refreshExp) = _tokens.CreateRefreshToken();
        await _repo.UpsertRefreshTokenAsync(user.Id, refreshHash, refreshExp, deviceInfo, ct);

        return (new AuthResponse(
                user.Id,
                user.Name,
                user.Mobile,
                user.Email,
                access,
                accessExp,
                refresh,
                refreshExp),
            null,
            "Registration successful.");
    }

    public async Task<(AuthResponse? Response, string? ErrorCode, string Message)>
        LoginAsync(LoginRequest request, string? deviceInfo, CancellationToken ct)
    {
        var user = await _repo.GetUserByMobileAsync(request.Mobile, ct);
        if (user is null)
            return (null, ErrorCodes.InvalidCredentials, "Invalid mobile or password.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            return (null, ErrorCodes.InvalidCredentials, "Invalid mobile or password.");

        var (access, accessExp) = _tokens.CreateAccessToken(user);
        var (refresh, refreshHash, refreshExp) = _tokens.CreateRefreshToken();
        await _repo.UpsertRefreshTokenAsync(user.Id, refreshHash, refreshExp, deviceInfo, ct);

        return (new AuthResponse(
                user.Id,
                user.Name,
                user.Mobile,
                user.Email,
                access,
                accessExp,
                refresh,
                refreshExp),
            null,
            "Login successful.");
    }

    public async Task<(AuthResponse? Response, string? ErrorCode, string Message)>
        RefreshAsync(Guid userId, string refreshToken, string? deviceInfo, CancellationToken ct)
    {
        var refreshHash = _tokens.HashRefreshToken(refreshToken);

        var user = await _repo.GetUserByIdAsync(userId, ct);
        if (user is null)
            return (null, ErrorCodes.Unauthorized, "User not found.");

        var tokenRow = await _repo.GetActiveRefreshTokenAsync(userId, refreshHash, ct);
        if (tokenRow is null)
            return (null, ErrorCodes.Unauthorized, "Refresh token is invalid or expired.");

        await _repo.RevokeRefreshTokenAsync(tokenRow.Id, ct);

        var (access, accessExp) = _tokens.CreateAccessToken(user);
        var (newRefresh, newRefreshHash, newRefreshExp) = _tokens.CreateRefreshToken();
        await _repo.UpsertRefreshTokenAsync(user.Id, newRefreshHash, newRefreshExp, deviceInfo, ct);

        return (new AuthResponse(
                user.Id,
                user.Name,
                user.Mobile,
                user.Email,
                access,
                accessExp,
                newRefresh,
                newRefreshExp),
            null,
            "Token refreshed.");
    }
}