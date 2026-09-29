using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Domain.Entities;

namespace TexasMediaDart.Identity.Infrastructure.Persistence;

public sealed class UserInvitationRepository : IUserInvitationRepository
{
    private readonly string _connectionString;

    public UserInvitationRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
    }

    public async Task<UserInvitation> CreateAsync(
        string email,
        Guid organizationId,
        Guid invitedByIdentityUserId,
        string tokenHash,
        DateTime expiresUtc,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_UserInvitations_Create",
            parameters: new
            {
                Email = email,
                OrganizationId = organizationId,
                InvitedByIdentityUserId = invitedByIdentityUserId,
                TokenHash = tokenHash,
                ExpiresUtc = expiresUtc
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleAsync<UserInvitation>(command);
    }

    public async Task<UserInvitation> ResendAsync(
        Guid invitationId,
        Guid organizationId,
        string tokenHash,
        DateTime expiresUtc,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_UserInvitations_Resend",
            parameters: new
            {
                InvitationId = invitationId,
                OrganizationId = organizationId,
                TokenHash = tokenHash,
                ExpiresUtc = expiresUtc
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleAsync<UserInvitation>(command);
    }

    public async Task<UserInvitation?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_UserInvitations_GetByTokenHash",
            parameters: new
            {
                TokenHash = tokenHash
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<UserInvitation>(command);
    }

    public async Task<UserInvitation?> GetPendingByEmailAsync(
        string email,
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_UserInvitations_GetPendingByEmail",
            parameters: new
            {
                Email = email,
                OrganizationId = organizationId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<UserInvitation>(command);
    }

    public async Task<IReadOnlyList<PendingUserInvitation>> GetPendingAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_UserInvitations_GetPending",
            parameters: new
            {
                OrganizationId = organizationId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var invitations =
            await connection.QueryAsync<PendingUserInvitation>(
                command);

        return invitations.AsList();
    }
    public async Task<AcceptUserInvitationIdentityResult> AcceptIdentityAsync(
        string tokenHash,
        string passwordHash,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_UserInvitations_AcceptIdentity",
            parameters: new
            {
                TokenHash = tokenHash,
                PasswordHash = passwordHash
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleAsync<AcceptUserInvitationIdentityResult>(
                command);
    }

    public async Task<FinalizeUserInvitationResult> FinalizeAsync(
        Guid invitationId,
        Guid identityUserId,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_UserInvitations_Finalize",
            parameters: new
            {
                InvitationId = invitationId,
                IdentityUserId = identityUserId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleAsync<FinalizeUserInvitationResult>(
                command);
    }
}