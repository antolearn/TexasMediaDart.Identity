using TexasMediaDart.Identity.Application.Features.Users.Models;

namespace TexasMediaDart.Identity.Application.Features.Users.Queries.LookupUsers;

public sealed record LookupUsersQuery(
    IReadOnlyCollection<Guid> UserIds);