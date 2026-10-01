using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TexasMediaDart.Identity.Application.Abstractions.Email;
using TexasMediaDart.Identity.Application.Abstractions.Notifications;
using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Application.Abstractions.Security;
using TexasMediaDart.Identity.Infrastructure.Email;
using TexasMediaDart.Identity.Infrastructure.Notifications;
using TexasMediaDart.Identity.Infrastructure.Persistence;
using TexasMediaDart.Identity.Infrastructure.Security;

namespace TexasMediaDart.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //
        // Persistence
        //

        services.AddScoped<
            IUserRepository,
            UserRepository>();

        services.AddScoped<
            IUserInvitationRepository,
            UserInvitationRepository>();

        services.AddScoped<
            IRefreshTokenRepository,
            RefreshTokenRepository>();

        services.AddScoped<
            ITermsRepository,
            TermsRepository>();

        services.AddScoped<
            IEmailVerificationRepository,
            EmailVerificationRepository>();

        //
        // Security
        //

        services.AddScoped<
            IPasswordHasher,
            PasswordHasher>();

        services.AddScoped<
            ITokenService,
            TokenService>();

        services.AddScoped<
            IRefreshTokenService,
            RefreshTokenService>();

        services.AddScoped<
            IEmailVerificationTokenGenerator,
            EmailVerificationTokenGenerator>();

        //
        // Email verification
        //

        services.Configure<EmailOptions>(
            configuration.GetSection(
                EmailOptions.SectionName));

        services.AddScoped<
            IEmailVerificationLinkBuilder,
            EmailVerificationLinkBuilder>();

        //
        // Notification service
        //

        services.Configure<NotificationOptions>(
            configuration.GetSection(
                NotificationOptions.SectionName));

        var notificationBaseUrl =
            configuration["NotificationService:BaseUrl"]
            ?? throw new InvalidOperationException(
                "NotificationService:BaseUrl is not configured.");

        if (!Uri.TryCreate(
                notificationBaseUrl,
                UriKind.Absolute,
                out var notificationBaseUri))
        {
            throw new InvalidOperationException(
                "NotificationService:BaseUrl must be a valid absolute URL.");
        }

        services.AddHttpClient<
            INotificationClient,
            NotificationClient>(
            client =>
            {
                client.BaseAddress =
                    notificationBaseUri;
            });

        return services;
    }
}
