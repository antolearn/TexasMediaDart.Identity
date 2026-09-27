namespace TexasMediaDart.Identity.Api.Models.Users;

public sealed class UserEmailSearchRequest
{
    public IReadOnlyCollection<Guid> UserIds { get; init; }
        = Array.Empty<Guid>();

    public string? Email { get; init; }
}