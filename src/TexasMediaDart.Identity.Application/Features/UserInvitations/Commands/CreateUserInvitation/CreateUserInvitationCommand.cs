namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.CreateUserInvitation;

public sealed record CreateUserInvitationCommand(
    string Email,
    Guid OrganizationId,
    Guid InvitedByIdentityUserId);