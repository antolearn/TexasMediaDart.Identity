namespace TexasMediaDart.Identity.Domain.Entities;

public sealed class User
{
    public Guid UserId { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public bool IsDeleted { get; private set; }

    public bool IsEmailVerified { get; private set; }

    public DateTime CreatedDateUtc { get; private set; }

    public DateTime? ModifiedDateUtc { get; private set; }

    private User()
    {
    }

    public User(
        Guid userId,
        string email,
        string passwordHash,
        bool isActive,
        bool isDeleted,
        bool isEmailVerified,
        DateTime createdDateUtc,
        DateTime? modifiedDateUtc)
    {
        UserId = userId;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = isActive;
        IsDeleted = isDeleted;
        IsEmailVerified = isEmailVerified;
        CreatedDateUtc = createdDateUtc;
        ModifiedDateUtc = modifiedDateUtc;
    }
}