using TexasMediaDart.Identity.Application.Abstractions.Persistence;

namespace TexasMediaDart.Identity.Application.Features.UserInvitations.Queries.ValidateUserInvitation;

public sealed class ValidateUserInvitationQueryHandler
{
    private readonly IUserInvitationRepository _userInvitationRepository;

    public ValidateUserInvitationQueryHandler(
        IUserInvitationRepository userInvitationRepository)
    {
        _userInvitationRepository = userInvitationRepository;
    }

    public async Task<ValidateUserInvitationResult> HandleAsync(
        ValidateUserInvitationQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.Token))
        {
            return Invalid("invalid");
        }

        var tokenHash =
            UserInvitationTokenService.HashToken(
                query.Token.Trim());

        var invitation =
            await _userInvitationRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

        if (invitation is null)
        {
            return Invalid("invalid");
        }

        if (invitation.RevokedUtc is not null)
        {
            return Invalid("revoked");
        }

        if (invitation.AcceptedUtc is not null)
        {
            return Invalid("accepted");
        }

        if (invitation.ExpiresUtc <= DateTime.UtcNow)
        {
            return Invalid("expired");
        }

        return new ValidateUserInvitationResult(
            IsValid: true,
            Email: invitation.Email,
            OrganizationId: invitation.OrganizationId,
            ExpiresUtc: invitation.ExpiresUtc,
            Reason: null);
    }

    private static ValidateUserInvitationResult Invalid(
        string reason)
    {
        return new ValidateUserInvitationResult(
            IsValid: false,
            Email: null,
            OrganizationId: null,
            ExpiresUtc: null,
            Reason: reason);
    }
}