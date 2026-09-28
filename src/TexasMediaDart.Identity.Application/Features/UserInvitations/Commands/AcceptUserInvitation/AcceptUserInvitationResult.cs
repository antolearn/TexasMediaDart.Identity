namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.AcceptUserInvitation;

public sealed record AcceptUserInvitationResult(
    Guid InvitationId,
    Guid IdentityUserId,
    string Email,
    Guid OrganizationId,
    DateTime IdentityCreatedUtc);