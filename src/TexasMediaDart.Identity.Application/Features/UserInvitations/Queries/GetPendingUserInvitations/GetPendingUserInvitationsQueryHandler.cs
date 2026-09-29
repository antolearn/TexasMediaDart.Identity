using TexasMediaDart.Identity.Application.Abstractions.Persistence;

namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Queries.GetPendingUserInvitations;

public sealed class GetPendingUserInvitationsQueryHandler
{
    private readonly IUserInvitationRepository
        _userInvitationRepository;

    public GetPendingUserInvitationsQueryHandler(
        IUserInvitationRepository userInvitationRepository)
    {
        _userInvitationRepository =
            userInvitationRepository;
    }

    public async Task<IReadOnlyList<PendingUserInvitationResult>>
        HandleAsync(
            GetPendingUserInvitationsQuery query,
            CancellationToken cancellationToken = default)
    {
        if (query.OrganizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization id is required.",
                nameof(query));
        }

        var invitations =
            await _userInvitationRepository.GetPendingAsync(
                query.OrganizationId,
                cancellationToken);

        return invitations
            .Select(invitation =>
                new PendingUserInvitationResult(
                    invitation.InvitationId,
                    invitation.Email,
                    invitation.OrganizationId,
                    invitation.InvitedByIdentityUserId,
                    invitation.ExpiresUtc,
                    invitation.CreatedUtc))
            .ToList();
    }
}