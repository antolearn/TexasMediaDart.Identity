namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Queries.GetPendingUserInvitations;

public sealed record GetPendingUserInvitationsQuery(
    Guid OrganizationId);