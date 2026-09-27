namespace TexasMediaDart.Identity.Application.Features.Users.Queries.SearchUsersByEmailAndIds;

public sealed record SearchUsersByEmailAndIdsQuery(
    IReadOnlyCollection<Guid> UserIds,
    string? Email);