namespace TexasMediaDart.Identity.Api.Models.UserInvitations;

public sealed class CreateUserInvitationRequest
{
    public string Email { get; init; } = string.Empty;

    public Guid OrganizationId { get; init; }
}