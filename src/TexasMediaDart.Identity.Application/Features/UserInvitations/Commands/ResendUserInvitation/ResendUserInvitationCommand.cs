namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.ResendUserInvitation;

public sealed record ResendUserInvitationCommand(
    Guid InvitationId,
    Guid OrganizationId);