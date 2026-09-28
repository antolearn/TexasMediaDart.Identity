namespace TexasMediaDart.Identity.Application.Abstractions.Persistence;

public sealed record FinalizeUserInvitationResult(
    Guid InvitationId,
    string Email,
    Guid OrganizationId,
    Guid CreatedIdentityUserId,
    DateTime IdentityCreatedUtc,
    DateTime AcceptedUtc);