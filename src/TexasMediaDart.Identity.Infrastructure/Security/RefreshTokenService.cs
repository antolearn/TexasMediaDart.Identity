using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using TexasMediaDart.Identity.Application.Abstractions.Security;

namespace TexasMediaDart.Identity.Infrastructure.Security;

public sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IConfiguration _configuration;

    public RefreshTokenService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public RefreshTokenResult GenerateToken()
    {
        var expiresDays =
            int.TryParse(
                _configuration["RefreshToken:ExpiresDays"],
                out var parsedDays)
                ? parsedDays
                : 30;

        var bytes =
            RandomNumberGenerator.GetBytes(64);

        var token =
            Convert.ToBase64String(bytes);

        var tokenHash =
            HashToken(token);

        var expiresAtUtc =
            DateTime.UtcNow.AddDays(expiresDays);

        return new RefreshTokenResult(
            token,
            tokenHash,
            expiresAtUtc);
    }

    public string HashToken(string token)
    {
        var tokenBytes =
            Encoding.UTF8.GetBytes(token);

        var hashBytes =
            SHA256.HashData(tokenBytes);

        return Convert.ToBase64String(hashBytes);
    }
    


    
}