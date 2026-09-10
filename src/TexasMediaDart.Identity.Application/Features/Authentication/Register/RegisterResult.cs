namespace TexasMediaDart.Identity.Application.Features.Authentication.Register;

public sealed record RegisterResult(
    Guid UserId,
    string Email);