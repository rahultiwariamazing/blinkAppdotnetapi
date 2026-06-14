using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlinkDemoApi.Application.Auth.Interfaces;
using BlinkDemoApi.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlinkDemoApi.Infrastructure.Security;

/// <summary>
/// Creates JWT access tokens and secure refresh tokens.
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public (string Token, DateTime ExpiresAt) CreateAccessToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("mobile", user.Mobile),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Roles are not used now, but we keep it scalable.
        // Later you can add: claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    public (string RefreshToken, string RefreshTokenHash, DateTime ExpiresAt) CreateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var refreshToken = Convert.ToBase64String(bytes);
        var refreshHash = HashRefreshToken(refreshToken);
        var expiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenDays);
        return (refreshToken, refreshHash, expiresAt);
    }

    public string HashRefreshToken(string refreshToken)
    {
        var bytes = Encoding.UTF8.GetBytes(refreshToken);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}

/// <summary>
/// Jwt config read from appsettings.json.
/// </summary>
public sealed class JwtOptions
{
    public string Issuer { get; set; } = "BlinkDemoApi";
    public string Audience { get; set; } = "BlinkDemoApiClients";

    /// <summary>
    /// Keep this key secret. In production store it in environment variable / KeyVault.
    /// </summary>
    public string SigningKey { get; set; } = "PLEASE_CHANGE_ME_TO_A_LONG_RANDOM_SECRET";

    public int AccessTokenMinutes { get; set; } = 60;
    public int RefreshTokenDays { get; set; } = 30;
}
