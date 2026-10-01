using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using TexasMediaDart.Identity.Application.Abstractions.Notifications;

namespace TexasMediaDart.Identity.Infrastructure.Notifications;

public sealed class NotificationClient : INotificationClient
{
    private const string ApiKeyHeaderName = "X-API-Key";

    private readonly HttpClient _httpClient;
    private readonly NotificationOptions _options;

    public NotificationClient(
        HttpClient httpClient,
        IOptions<NotificationOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task SendEmailVerificationAsync(
        string recipientEmail,
        string verificationUrl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException(
                "NotificationService:ApiKey is not configured.");
        }

        var notification =
            new EmailVerificationNotificationRequest(
                recipientEmail,
                verificationUrl);

        using var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                "internal/notifications/email-verification")
            {
                Content =
                    JsonContent.Create(notification)
            };

        request.Headers.Add(
            ApiKeyHeaderName,
            _options.ApiKey);

        using var response =
            await _httpClient.SendAsync(
                request,
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var responseBody =
                await response.Content.ReadAsStringAsync(
                    cancellationToken);

            throw new InvalidOperationException(
                $"Notification service failed to send the " +
                $"email verification notification. " +
                $"StatusCode={(int)response.StatusCode}. " +
                $"Response={responseBody}");
        }
    }

    private sealed record EmailVerificationNotificationRequest(
        string RecipientEmail,
        string VerificationUrl);
}