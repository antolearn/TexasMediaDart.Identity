using TexasMediaDart.Identity.Application.Features.UserInvitations;
using TexasMediaDart.Identity.Application.Abstractions.Persistence;

namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.CreateUserInvitation;

public sealed class CreateUserInvitationCommandHandler
{
    private const int TokenSizeInBytes = 32;
    private static readonly TimeSpan InvitationLifetime = TimeSpan.FromDays(7);

    private readonly IUserRepository _userRepository;
    private readonly IUserInvitationRepository _userInvitationRepository;

    public CreateUserInvitationCommandHandler(
        IUserRepository userRepository,
        IUserInvitationRepository userInvitationRepository)
    {
        _userRepository = userRepository;
        _userInvitationRepository = userInvitationRepository;
    }

    public async Task<CreateUserInvitationResult> HandleAsync(
        CreateUserInvitationCommand command,
        CancellationToken cancellationToken = default)
    {
        var email = command.Email.Trim();

        var existingUser = await _userRepository.GetByEmailAsync(
            email,
            cancellationToken);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        var existingInvitation =
            await _userInvitationRepository.GetPendingByEmailAsync(
                email,
                command.OrganizationId,
                cancellationToken);

        if (existingInvitation is not null)
        {
            throw new InvalidOperationException(
                "A pending invitation already exists for this email and organization.");
        }

        var invitationToken =
            UserInvitationTokenService.GenerateToken();

        var tokenHash =
            UserInvitationTokenService.HashToken(invitationToken);
        var expiresUtc = DateTime.UtcNow.Add(InvitationLifetime);

        var invitation = await _userInvitationRepository.CreateAsync(
            email,
            command.OrganizationId,
            command.InvitedByIdentityUserId,
            tokenHash,
            expiresUtc,
            cancellationToken);

        return new CreateUserInvitationResult(
            invitation.InvitationId,
            invitation.Email,
            invitation.OrganizationId,
            invitation.ExpiresUtc,
            invitationToken);
    }

}