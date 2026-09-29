namespace TexasMediaDart.Identity.Application.Abstractions.Persistence;

public sealed record PendingUserInvitation(
    Guid InvitationId,
    string Email,
    Guid OrganizationId,
    Guid InvitedByIdentityUserId,
    DateTime ExpiresUtc,
    DateTime CreatedUtc);