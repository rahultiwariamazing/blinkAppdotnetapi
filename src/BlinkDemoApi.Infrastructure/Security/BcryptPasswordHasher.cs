using BlinkDemoApi.Application.Auth.Interfaces;

namespace BlinkDemoApi.Infrastructure.Security;

/// <summary>
/// BCrypt password hasher implementation.
/// BCrypt automatically includes salt and is a safe default for many apps.
/// </summary>
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string passwordHash)
        => BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
