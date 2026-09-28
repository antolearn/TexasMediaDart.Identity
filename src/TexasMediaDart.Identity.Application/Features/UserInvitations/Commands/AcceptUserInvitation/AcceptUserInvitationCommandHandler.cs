using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Application.Abstractions.Security;

namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.AcceptUserInvitation;

public sealed class AcceptUserInvitationCommandHandler
{
    private readonly IUserInvitationRepository _userInvitationRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AcceptUserInvitationCommandHandler(
        IUserInvitationRepository userInvitationRepository,
        IPasswordHasher passwordHasher)
    {
        _userInvitationRepository = userInvitationRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<AcceptUserInvitationResult> HandleAsync(
        AcceptUserInvitationCommand command,
        CancellationToken cancellationToken = default)
    {
        var token = command.Token.Trim();

        var tokenHash =
            UserInvitationTokenService.HashToken(token);

        var passwordHash =
            _passwordHasher.Hash(command.Password);

        var result =
            await _userInvitationRepository.AcceptIdentityAsync(
                tokenHash,
                passwordHash,
                cancellationToken);

        return new AcceptUserInvitationResult(
            result.InvitationId,
            result.CreatedIdentityUserId,
            result.Email,
            result.OrganizationId,
            result.IdentityCreatedUtc);
    }
}