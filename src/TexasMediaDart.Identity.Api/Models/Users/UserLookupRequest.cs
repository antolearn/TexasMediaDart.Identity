namespace TexasMediaDart.Identity.Api.Models.Users;

public sealed class UserLookupRequest
{
    public IReadOnlyCollection<Guid> UserIds { get; init; }
        = Array.Empty<Guid>();
}