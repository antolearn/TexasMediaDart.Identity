namespace TexasMediaDart.Identity.Domain.Entities;

public sealed class RefreshToken
{
    public Guid RefreshTokenId { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresDateUtc { get; set; }

    public DateTime CreatedDateUtc { get; set; }

    public DateTime? RevokedDateUtc { get; set; }

    public bool IsRevoked { get; set; }

    public string? ReplacedByTokenHash { get; set; }
}