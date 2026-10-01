namespace TexasMediaDart.Identity.Infrastructure.Notifications;

public sealed class NotificationOptions
{
    public const string SectionName = "NotificationService";

    public string BaseUrl { get; init; } = string.Empty;

    public string ApiKey { get; init; } = string.Empty;
}