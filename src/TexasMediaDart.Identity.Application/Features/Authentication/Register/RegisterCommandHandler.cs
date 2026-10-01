using TexasMediaDart.Identity.Application.Abstractions.Email;
using TexasMediaDart.Identity.Application.Abstractions.Notifications;
using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Application.Abstractions.Security;

namespace TexasMediaDart.Identity.Application.Features.Authentication.Register;

public sealed class RegisterCommandHandler
{
    private static readonly TimeSpan VerificationTokenLifetime =
        TimeSpan.FromHours(24);

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailVerificationTokenGenerator
        _emailVerificationTokenGenerator;
    private readonly IEmailVerificationLinkBuilder
        _emailVerificationLinkBuilder;
    private readonly INotificationClient _notificationClient;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IEmailVerificationTokenGenerator emailVerificationTokenGenerator,
        IEmailVerificationLinkBuilder emailVerificationLinkBuilder,
        INotificationClient notificationClient)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _emailVerificationTokenGenerator =
            emailVerificationTokenGenerator;
        _emailVerificationLinkBuilder =
            emailVerificationLinkBuilder;
        _notificationClient = notificationClient;
    }

    public async Task<RegisterResult> HandleAsync(
        RegisterCommand command,
        CancellationToken cancellationToken = default)
    {
        // ------------------------------------------------------------
        // Check whether the email already exists
        // ------------------------------------------------------------

        var existingUser =
            await _userRepository.GetByEmailAsync(
                command.Email,
                cancellationToken);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "A user with this email already exists.");
        }

        // ------------------------------------------------------------
        // Hash password
        // ------------------------------------------------------------

        var passwordHash =
            _passwordHasher.Hash(command.Password);

        // ------------------------------------------------------------
        // Generate email verification token
        //
        // Generate() returns:
        //   Token     -> raw token sent to the user
        //   TokenHash -> SHA-256 hash persisted in the database
        // ------------------------------------------------------------

        var verificationToken =
            _emailVerificationTokenGenerator.Generate();

        var verificationExpiresUtc =
            DateTime.UtcNow.Add(
                VerificationTokenLifetime);

        // ------------------------------------------------------------
        // Create user
        //
        // The database receives only the token hash.
        //
        // User creation, Terms acceptance, and verification-token
        // persistence occur in one database transaction.
        // ------------------------------------------------------------

        var user =
            await _userRepository.CreateAsync(
                command.Email,
                passwordHash,
                verificationToken.TokenHash,
                verificationExpiresUtc,
                cancellationToken);

        // ------------------------------------------------------------
        // Build verification URL
        //
        // The raw token is used only to construct the verification URL.
        // It is never persisted.
        // ------------------------------------------------------------

        var verificationUrl =
            _emailVerificationLinkBuilder.Build(
                verificationToken.Token);

        // ------------------------------------------------------------
        // Send verification notification
        //
        // Identity owns:
        //   - user registration
        //   - verification token
        //   - verification URL
        //
        // Notification owns:
        //   - email template
        //   - email subject
        //   - ACS delivery
        // ------------------------------------------------------------

        await _notificationClient.SendEmailVerificationAsync(
            user.Email,
            verificationUrl,
            cancellationToken);

        // ------------------------------------------------------------
        // Return registration result
        // ------------------------------------------------------------

        return new RegisterResult(
            user.UserId,
            user.Email);
    }
}