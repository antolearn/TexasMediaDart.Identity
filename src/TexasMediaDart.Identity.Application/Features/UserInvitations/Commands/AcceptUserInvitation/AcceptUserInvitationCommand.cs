namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.AcceptUserInvitation;

public sealed record AcceptUserInvitationCommand(
    string Token,
    string Password,
    string ConfirmPassword);