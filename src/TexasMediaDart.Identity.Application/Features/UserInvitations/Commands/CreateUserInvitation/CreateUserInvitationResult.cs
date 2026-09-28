namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.CreateUserInvitation;

public sealed record CreateUserInvitationResult(
    Guid InvitationId,
    string Email,
    Guid OrganizationId,
    DateTime ExpiresUtc,
    string InvitationToken);