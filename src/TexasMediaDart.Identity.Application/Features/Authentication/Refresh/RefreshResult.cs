namespace TexasMediaDart.Identity.Application.Features.Authentication.Refresh;

public sealed record RefreshResult(
    Guid UserId,
    string Email,
    string AccessToken,
    DateTime ExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);