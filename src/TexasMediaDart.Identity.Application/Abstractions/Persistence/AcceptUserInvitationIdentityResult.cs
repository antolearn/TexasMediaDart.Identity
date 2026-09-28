namespace TexasMediaDart.Identity.Application.Abstractions.Persistence;

public sealed record AcceptUserInvitationIdentityResult(
    Guid InvitationId,
    string Email,
    Guid OrganizationId,
    Guid CreatedIdentityUserId,
    DateTime IdentityCreatedUtc,
    DateTime ExpiresUtc);