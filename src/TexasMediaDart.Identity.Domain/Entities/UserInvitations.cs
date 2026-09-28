namespace TexasMediaDart.Identity.Domain.Entities;

public sealed class UserInvitation
{
    public Guid InvitationId { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public Guid OrganizationId { get; private set; }

    public Guid InvitedByIdentityUserId { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public DateTime ExpiresUtc { get; private set; }

    public DateTime? AcceptedUtc { get; private set; }

    public DateTime? RevokedUtc { get; private set; }

    public DateTime CreatedUtc { get; private set; }

    private UserInvitation()
    {
    }

    public UserInvitation(
        Guid invitationId,
        string email,
        Guid organizationId,
        Guid invitedByIdentityUserId,
        string tokenHash,
        DateTime expiresUtc,
        DateTime? acceptedUtc,
        DateTime? revokedUtc,
        DateTime createdUtc)
    {
        InvitationId = invitationId;
        Email = email;
        OrganizationId = organizationId;
        InvitedByIdentityUserId = invitedByIdentityUserId;
        TokenHash = tokenHash;
        ExpiresUtc = expiresUtc;
        AcceptedUtc = acceptedUtc;
        RevokedUtc = revokedUtc;
        CreatedUtc = createdUtc;
    }
}