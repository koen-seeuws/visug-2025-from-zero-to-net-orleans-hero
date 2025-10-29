using Microsoft.AspNetCore.Identity;

namespace TheCodeKitchen.Infrastructure.Security;

public interface IPasswordHashingService
{
    string HashPassword(string password);
    bool VerifyHashedPassword(string hashedPassword, string password);
}

public sealed class PasswordHashingService : IPasswordHashingService
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    public string HashPassword(string password)
        => _passwordHasher.HashPassword(string.Empty, password);

    public bool VerifyHashedPassword(string hashedPassword, string password)
        => _passwordHasher.VerifyHashedPassword(string.Empty, hashedPassword, password) ==
           PasswordVerificationResult.Success;
}