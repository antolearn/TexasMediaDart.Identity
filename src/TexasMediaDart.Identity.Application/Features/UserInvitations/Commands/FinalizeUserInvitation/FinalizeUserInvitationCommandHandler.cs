using TexasMediaDart.Identity.Application.Abstractions.Persistence;

namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.FinalizeUserInvitation;

public sealed class FinalizeUserInvitationCommandHandler
{
    private readonly IUserInvitationRepository _userInvitationRepository;

    public FinalizeUserInvitationCommandHandler(
        IUserInvitationRepository userInvitationRepository)
    {
        _userInvitationRepository = userInvitationRepository;
    }

    public async Task<FinalizeUserInvitationResult> HandleAsync(
        FinalizeUserInvitationCommand command,
        CancellationToken cancellationToken = default)
    {
        var result =
            await _userInvitationRepository.FinalizeAsync(
                command.InvitationId,
                command.IdentityUserId,
                cancellationToken);

        return new FinalizeUserInvitationResult(
            result.InvitationId,
            result.CreatedIdentityUserId,
            result.Email,
            result.OrganizationId,
            result.IdentityCreatedUtc,
            result.AcceptedUtc);
    }
}