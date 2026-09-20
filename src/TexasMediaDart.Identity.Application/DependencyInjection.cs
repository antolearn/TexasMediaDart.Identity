using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TexasMediaDart.Identity.Application.Features.Authentication.Login;
using TexasMediaDart.Identity.Application.Features.Authentication.Logout;
using TexasMediaDart.Identity.Application.Features.Authentication.Refresh;
using TexasMediaDart.Identity.Application.Features.Authentication.Register;
using TexasMediaDart.Identity.Application.Features.Users.Queries.LookupUsers;

namespace TexasMediaDart.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<RegisterCommandHandler>();
        services.AddScoped<RefreshCommandHandler>();
        services.AddScoped<LogoutCommandHandler>();

        services.AddScoped<LookupUsersQueryHandler>();

        services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();

        return services;
    }
}