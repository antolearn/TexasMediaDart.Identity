using System.Security.Cryptography;
using System.Text;
using TexasMediaDart.Identity.Application.Abstractions.Persistence;

namespace TexasMediaDart.Identity.Application.Features.Authentication.VerifyEmail;

public sealed class VerifyEmailCommandHandler
{
    private readonly IEmailVerificationRepository
        _emailVerificationRepository;

    public VerifyEmailCommandHandler(
        IEmailVerificationRepository emailVerificationRepository)
    {
        _emailVerificationRepository =
            emailVerificationRepository;
    }

    public async Task<EmailVerificationResult> HandleAsync(
        VerifyEmailCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(command.Token);

        var tokenHash = HashToken(command.Token);

        return await _emailVerificationRepository.VerifyAsync(
            tokenHash,
            cancellationToken);
    }

    private static string HashToken(string token)
    {
        var tokenBytes =
            Encoding.UTF8.GetBytes(token);

        var hashBytes =
            SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}