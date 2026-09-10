using Microsoft.Extensions.DependencyInjection;
using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Application.Abstractions.Security;
using TexasMediaDart.Identity.Infrastructure.Persistence;
using TexasMediaDart.Identity.Infrastructure.Security;

namespace TexasMediaDart.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        return services;
    }
}