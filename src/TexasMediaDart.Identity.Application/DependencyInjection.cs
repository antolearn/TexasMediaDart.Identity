using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TexasMediaDart.Identity.Application.Features.Authentication.Login;
using TexasMediaDart.Identity.Application.Features.Authentication.Logout;
using TexasMediaDart.Identity.Application.Features.Authentication.Refresh;
using TexasMediaDart.Identity.Application.Features.Authentication.Register;
using TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.AcceptUserInvitation;
using TexasMediaDart.Identity.Application.Features.UserInvitations.Commands.CreateUserInvitation;
using TexasMediaDart.Identity.Application.Features.UserInvitations.Queries.ValidateUserInvitation;
using TexasMediaDart.Identity.Application.Features.Users.Queries.GetUserByEmail;
using TexasMediaDart.Identity.Application.Features.Users.Queries.LookupUsers;
using TexasMediaDart.Identity.Application.Features.Users.Queries.SearchUsersByEmailAndIds;

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
        services.AddScoped<SearchUsersByEmailAndIdsQueryHandler>();
        services.AddScoped<GetUserByEmailQueryHandler>();

        services.AddScoped<CreateUserInvitationCommandHandler>();
        services.AddScoped<AcceptUserInvitationCommandHandler>();
        services.AddScoped<ValidateUserInvitationQueryHandler>();

        services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();

        return services;
    }
}