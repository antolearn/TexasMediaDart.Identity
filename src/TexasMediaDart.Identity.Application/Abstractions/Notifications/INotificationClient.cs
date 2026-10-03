namespace TexasMediaDart.Identity.Application.Abstractions.Notifications;

public interface INotificationClient
{
    Task SendEmailVerificationAsync(
        string recipientEmail,
        string verificationUrl,
        CancellationToken cancellationToken = default);
}