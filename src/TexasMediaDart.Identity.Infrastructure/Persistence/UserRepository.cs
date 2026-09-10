using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Domain.Entities;

namespace TexasMediaDart.Identity.Infrastructure.Persistence;

public sealed class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
    }

    public async Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_Users_GetById",
            parameters: new
            {
                UserId = userId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<User>(
                command);
    }

    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_Users_GetByEmail",
            parameters: new
            {
                Email = email
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<User>(command);
    }

    public async Task<User> CreateAsync(
        string email,
        string passwordHash,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_Users_Create",
            parameters: new
            {
                Email = email,
                PasswordHash = passwordHash
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var user = await connection.QuerySingleAsync<User>(command);

        return user;
    }
}