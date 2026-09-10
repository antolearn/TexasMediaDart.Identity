namespace TexasMediaDart.Identity.Application.Features.Authentication.Logout;

public sealed record LogoutCommand(
    string RefreshToken);