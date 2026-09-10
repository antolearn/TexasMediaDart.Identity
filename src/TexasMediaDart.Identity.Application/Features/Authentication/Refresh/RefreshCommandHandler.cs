using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Application.Abstractions.Security;

namespace TexasMediaDart.Identity.Application.Features.Authentication.Refresh;

public sealed class RefreshCommandHandler
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly ITokenService _tokenService;

    public RefreshCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IRefreshTokenService refreshTokenService,
        ITokenService tokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _refreshTokenService = refreshTokenService;
        _tokenService = tokenService;
    }

    public async Task<RefreshResult> HandleAsync(
        RefreshCommand command,
        CancellationToken cancellationToken = default)
    {
        var incomingTokenHash =
            _refreshTokenService.HashToken(
                command.RefreshToken);

        var existingToken =
            await _refreshTokenRepository.GetByTokenHashAsync(
                incomingTokenHash,
                cancellationToken);

        if (existingToken is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid refresh token.");
        }

        if (existingToken.IsRevoked)
        {
            throw new UnauthorizedAccessException(
                "Refresh token has been revoked.");
        }

        if (existingToken.IsRevoked)
        {
            await _refreshTokenRepository.RevokeFamilyAsync(
                existingToken.TokenHash,
                cancellationToken);

            throw new UnauthorizedAccessException(
                "Refresh token reuse detected.");
        }

        var user =
            await _userRepository.GetByIdAsync(
                existingToken.UserId,
                cancellationToken);

        if (user is null ||
            !user.IsActive ||
            user.IsDeleted)
        {
            throw new UnauthorizedAccessException(
                "Invalid refresh token.");
        }

        var newRefreshToken =
            _refreshTokenService.GenerateToken();

        await _refreshTokenRepository.RotateAsync(
            existingToken.TokenHash,
            newRefreshToken.TokenHash,
            newRefreshToken.ExpiresAtUtc,
            cancellationToken);

        var accessToken =
            _tokenService.GenerateToken(
                user.UserId,
                user.Email);

        return new RefreshResult(
            user.UserId,
            user.Email,
            accessToken.AccessToken,
            accessToken.ExpiresAtUtc,
            newRefreshToken.Token,
            newRefreshToken.ExpiresAtUtc);
    }

    
}