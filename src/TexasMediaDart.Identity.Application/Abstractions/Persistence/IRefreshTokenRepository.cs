using TexasMediaDart.Identity.Domain.Entities;

namespace TexasMediaDart.Identity.Application.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateAsync(
        Guid userId,
        string tokenHash,
        DateTime expiresDateUtc,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task<RefreshToken?> RevokeAsync(
        string tokenHash,
        string? replacedByTokenHash = null,
        CancellationToken cancellationToken = default);
    Task RevokeFamilyAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);
    
    Task<RefreshToken> RotateAsync(
        string currentTokenHash,
        string newTokenHash,
        DateTime newExpiresDateUtc,
        CancellationToken cancellationToken = default);
}