namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.FinalizeUserInvitation;

public sealed record FinalizeUserInvitationResult(
    Guid InvitationId,
    Guid IdentityUserId,
    string Email,
    Guid OrganizationId,
    DateTime IdentityCreatedUtc,
    DateTime AcceptedUtc);