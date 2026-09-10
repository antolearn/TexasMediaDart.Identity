namespace TexasMediaDart.Identity.Application.Abstractions.Security;

public sealed record RefreshTokenResult(
    string Token,
    string TokenHash,
    DateTime ExpiresAtUtc);

public interface IRefreshTokenService
{
    RefreshTokenResult GenerateToken();

    string HashToken(string token);
}