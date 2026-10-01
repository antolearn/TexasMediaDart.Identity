namespace TexasMediaDart.Identity.Application.Features.Authentication.VerifyEmail;

public sealed record EmailVerificationResult(
    Guid UserId,
    string Email,
    bool IsEmailVerified);