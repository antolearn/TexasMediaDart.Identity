using Microsoft.Extensions.Options;
using TexasMediaDart.Identity.Application.Abstractions.Email;

namespace TexasMediaDart.Identity.Infrastructure.Email;

public sealed class EmailVerificationLinkBuilder
    : IEmailVerificationLinkBuilder
{
    private readonly EmailOptions _options;

    public EmailVerificationLinkBuilder(
        IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    public string Build(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        if (string.IsNullOrWhiteSpace(_options.FrontendBaseUrl))
        {
            throw new InvalidOperationException(
                "Email frontend base URL is not configured.");
        }

        var baseUrl =
            _options.FrontendBaseUrl.TrimEnd('/');

        var encodedToken =
            Uri.EscapeDataString(token);

        return $"{baseUrl}/#/verify-email?token={encodedToken}";
    }
}