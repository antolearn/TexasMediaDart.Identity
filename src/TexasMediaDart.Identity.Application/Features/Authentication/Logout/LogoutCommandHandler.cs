using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Application.Abstractions.Security;

namespace TexasMediaDart.Identity.Application.Features.Authentication.Logout;

public sealed class LogoutCommandHandler
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService;

    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenService refreshTokenService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenService = refreshTokenService;
    }

    public async Task HandleAsync(
        LogoutCommand command,
        CancellationToken cancellationToken = default)
    {
        var tokenHash =
            _refreshTokenService.HashToken(
                command.RefreshToken);

        var existingToken =
            await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

        if (existingToken is null)
        {
            return;
        }

        if (existingToken.IsRevoked)
        {
            return;
        }

        await _refreshTokenRepository.RevokeAsync(
            tokenHash,
            null,
            cancellationToken);
    }
}