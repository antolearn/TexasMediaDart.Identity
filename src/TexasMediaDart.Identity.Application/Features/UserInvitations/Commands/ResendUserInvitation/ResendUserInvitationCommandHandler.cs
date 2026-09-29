using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Application.Features.UserInvitations;

namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.ResendUserInvitation;

public sealed class ResendUserInvitationCommandHandler
{
    private static readonly TimeSpan InvitationLifetime =
        TimeSpan.FromDays(7);

    private readonly IUserInvitationRepository
        _userInvitationRepository;

    public ResendUserInvitationCommandHandler(
        IUserInvitationRepository userInvitationRepository)
    {
        _userInvitationRepository =
            userInvitationRepository;
    }

    public async Task<ResendUserInvitationResult> HandleAsync(
        ResendUserInvitationCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.InvitationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Invitation id is required.",
                nameof(command));
        }

        if (command.OrganizationId == Guid.Empty)
        {
            throw new ArgumentException(
                "Organization id is required.",
                nameof(command));
        }

        var invitationToken =
            UserInvitationTokenService.GenerateToken();

        var tokenHash =
            UserInvitationTokenService.HashToken(
                invitationToken);

        var expiresUtc =
            DateTime.UtcNow.Add(InvitationLifetime);

        var invitation =
            await _userInvitationRepository.ResendAsync(
                command.InvitationId,
                command.OrganizationId,
                tokenHash,
                expiresUtc,
                cancellationToken);

        return new ResendUserInvitationResult(
            invitation.InvitationId,
            invitation.Email,
            invitation.OrganizationId,
            invitation.ExpiresUtc,
            invitationToken);
    }
}