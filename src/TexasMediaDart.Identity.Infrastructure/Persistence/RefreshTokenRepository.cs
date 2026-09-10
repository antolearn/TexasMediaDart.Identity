using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Domain.Entities;

namespace TexasMediaDart.Identity.Infrastructure.Persistence;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly string _connectionString;

    public RefreshTokenRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
    }

    public async Task<RefreshToken> CreateAsync(
        Guid userId,
        string tokenHash,
        DateTime expiresDateUtc,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_RefreshTokens_Create",
            parameters: new
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresDateUtc = expiresDateUtc
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<RefreshToken>(
            command);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_RefreshTokens_GetByTokenHash",
            parameters: new
            {
                TokenHash = tokenHash
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<RefreshToken>(
                command);
    }

    public async Task<RefreshToken?> RevokeAsync(
        string tokenHash,
        string? replacedByTokenHash = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_RefreshTokens_Revoke",
            parameters: new
            {
                TokenHash = tokenHash,
                ReplacedByTokenHash = replacedByTokenHash
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<RefreshToken>(
                command);
    }
    public async Task RevokeFamilyAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_RefreshTokens_RevokeFamily",
            parameters: new
            {
                TokenHash = tokenHash
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
    public async Task<RefreshToken> RotateAsync(
        string currentTokenHash,
        string newTokenHash,
        DateTime newExpiresDateUtc,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_RefreshTokens_Rotate",
            parameters: new
            {
                CurrentTokenHash = currentTokenHash,
                NewTokenHash = newTokenHash,
                NewExpiresDateUtc = newExpiresDateUtc
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<RefreshToken>(
            command);
    }
}