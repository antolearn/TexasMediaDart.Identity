namespace TexasMediaDart.Identity.Infrastructure.Email;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public string FrontendBaseUrl { get; init; } = string.Empty;
}