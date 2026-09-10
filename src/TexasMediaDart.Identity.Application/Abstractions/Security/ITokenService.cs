namespace TexasMediaDart.Identity.Application.Abstractions.Security;

public sealed record TokenResult(
    string AccessToken,
    DateTime ExpiresAtUtc);

public interface ITokenService
{
    TokenResult GenerateToken(
        Guid userId,
        string email);
}