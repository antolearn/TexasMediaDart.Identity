using TexasMediaDart.Identity.Domain.Entities;

namespace TexasMediaDart.Identity.Application.Abstractions.Persistence;

public interface IUserInvitationRepository
{
    Task<UserInvitation> CreateAsync(
        string email,
        Guid organizationId,
        Guid invitedByIdentityUserId,
        string tokenHash,
        DateTime expiresUtc,
        CancellationToken cancellationToken = default);
    Task<UserInvitation> ResendAsync(
        Guid invitationId,
        Guid organizationId,
        string tokenHash,
        DateTime expiresUtc,
        CancellationToken cancellationToken = default);

    Task<UserInvitation?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<UserInvitation?> GetPendingByEmailAsync(
        string email,
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PendingUserInvitation>> GetPendingAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);

    Task<AcceptUserInvitationIdentityResult> AcceptIdentityAsync(
        string tokenHash,
        string passwordHash,
        CancellationToken cancellationToken = default);
    Task<FinalizeUserInvitationResult> FinalizeAsync(
        Guid invitationId,
        Guid identityUserId,
        CancellationToken cancellationToken = default);
}