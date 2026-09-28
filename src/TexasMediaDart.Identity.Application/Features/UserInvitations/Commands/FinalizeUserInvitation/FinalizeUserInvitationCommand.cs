namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.FinalizeUserInvitation;

public sealed record FinalizeUserInvitationCommand(
    Guid InvitationId,
    Guid IdentityUserId);