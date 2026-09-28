namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Queries.ValidateUserInvitation;

public sealed record ValidateUserInvitationResult(
    bool IsValid,
    string? Email,
    Guid? OrganizationId,
    DateTime? ExpiresUtc,
    string? Reason);