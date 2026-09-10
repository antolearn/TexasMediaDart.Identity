using Microsoft.AspNetCore.Identity;
using TexasMediaDart.Identity.Application.Abstractions.Security;

namespace TexasMediaDart.Identity.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _passwordHasher = new();

    public string Hash(string password)
    {
        return _passwordHasher.HashPassword(
            user: null!,
            password: password);
    }

    public bool Verify(
        string password,
        string passwordHash)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            user: null!,
            hashedPassword: passwordHash,
            providedPassword: password);

        return result != PasswordVerificationResult.Failed;
    }
}