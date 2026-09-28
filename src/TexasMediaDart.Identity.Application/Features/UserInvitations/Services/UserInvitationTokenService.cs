using System.Security.Cryptography;
using System.Text;

namespace TexasMediaDart.Identity.Application.Features.UserInvitations;

public static class UserInvitationTokenService
{
    private const int TokenSizeInBytes = 32;

    public static string GenerateToken()
    {
        var tokenBytes =
            RandomNumberGenerator.GetBytes(TokenSizeInBytes);

        return Convert.ToBase64String(tokenBytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public static string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert
            .ToHexString(hashBytes)
            .ToLowerInvariant();
    }
}