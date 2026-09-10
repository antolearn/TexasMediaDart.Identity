namespace TexasMediaDart.Identity.Application.Features.Authentication.Login;

public sealed record LoginResult(
    Guid UserId,
    string Email,
    string AccessToken,
    DateTime ExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);