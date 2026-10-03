using System.Security.Cryptography;
using TexasMediaDart.Identity.Application.Abstractions.Security;

namespace TexasMediaDart.Identity.Infrastructure.Security;

public sealed class EmailVerificationTokenGenerator
    : IEmailVerificationTokenGenerator
{
    private const int TokenSizeInBytes = 32;

    public EmailVerificationToken Generate()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(
            TokenSizeInBytes);

        var token = ToBase64Url(tokenBytes);

        var hashBytes = SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(token));

        var tokenHash = Convert.ToHexString(hashBytes);

        return new EmailVerificationToken(
            token,
            tokenHash);
    }

    private static string ToBase64Url(byte[] bytes)
    {
        return Convert
            .ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}