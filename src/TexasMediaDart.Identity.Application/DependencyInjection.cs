using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TexasMediaDart.Identity.Application.Features.Authentication.Register;
using TexasMediaDart.Identity.Application.Features.Authentication.Login;
using TexasMediaDart.Identity.Application.Features.Authentication.Refresh;
using TexasMediaDart.Identity.Application.Features.Authentication.Logout;
namespace TexasMediaDart.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<RegisterCommandHandler>();

        services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();
        services.AddScoped<RefreshCommandHandler>();
        services.AddScoped<LogoutCommandHandler>();
        return services;
    }
}