namespace BlinkDemoApi.Application.Auth.Interfaces;

/// <summary>
/// Abstraction for hashing/verifying passwords.
/// Keeps your code testable and avoids locking into one algorithm.
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}
