namespace TexasMediaDart.Identity.Application.Features.Users.Models;

public sealed class UserLookupDto
{
    public Guid UserId { get; init; }

    public string Email { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public bool IsEmailVerified { get; init; }
}