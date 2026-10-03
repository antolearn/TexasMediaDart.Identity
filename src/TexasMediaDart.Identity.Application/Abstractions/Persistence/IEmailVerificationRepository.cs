using TexasMediaDart.Identity.Application.Features.Authentication.VerifyEmail;

namespace TexasMediaDart.Identity.Application.Abstractions.Persistence;

public interface IEmailVerificationRepository
{
    Task<EmailVerificationResult> VerifyAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);
}