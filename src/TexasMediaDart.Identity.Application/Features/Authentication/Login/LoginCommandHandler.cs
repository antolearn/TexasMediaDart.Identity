using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Application.Abstractions.Security;

namespace TexasMediaDart.Identity.Application.Features.Authentication.Login;

public sealed class LoginCommandHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<LoginResult> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        //------------------------------------------------------------
        // Find user
        //------------------------------------------------------------
        var user =
            await _userRepository.GetByEmailAsync(
                command.Email,
                cancellationToken);

        //------------------------------------------------------------
        // Do not reveal whether the account exists
        //------------------------------------------------------------
        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        //------------------------------------------------------------
        // Account must be active
        //------------------------------------------------------------
        if (!user.IsActive || user.IsDeleted)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        //------------------------------------------------------------
        // Validate password before revealing verification status
        //------------------------------------------------------------
        var isPasswordValid =
            _passwordHasher.Verify(
                command.Password,
                user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        //------------------------------------------------------------
        // Email must be verified before authentication tokens
        // can be issued.
        //------------------------------------------------------------
        if (!user.IsEmailVerified)
        {
            throw new EmailVerificationRequiredException();
        }

        //------------------------------------------------------------
        // Generate access token
        //------------------------------------------------------------
        var accessToken =
            _tokenService.GenerateToken(
                user.UserId,
                user.Email);

        //------------------------------------------------------------
        // Generate refresh token
        //------------------------------------------------------------
        var refreshToken =
            _refreshTokenService.GenerateToken();

        //------------------------------------------------------------
        // Persist refresh token
        //------------------------------------------------------------
        await _refreshTokenRepository.CreateAsync(
            user.UserId,
            refreshToken.TokenHash,
            refreshToken.ExpiresAtUtc,
            cancellationToken);

        //------------------------------------------------------------
        // Return authentication result
        //------------------------------------------------------------
        return new LoginResult(
            user.UserId,
            user.Email,
            accessToken.AccessToken,
            accessToken.ExpiresAtUtc,
            refreshToken.Token,
            refreshToken.ExpiresAtUtc);
    }
}