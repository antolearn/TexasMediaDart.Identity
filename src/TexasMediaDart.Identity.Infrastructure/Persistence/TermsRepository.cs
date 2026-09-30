using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Application.Features.Terms.GetCurrent;

namespace TexasMediaDart.Identity.Infrastructure.Persistence;

public sealed class TermsRepository : ITermsRepository
{
    private readonly string _connectionString;

    public TermsRepository(IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
    }

    public async Task<CurrentTermsResponse?> GetCurrentAsync(
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_Terms_GetCurrent",
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection
            .QuerySingleOrDefaultAsync<CurrentTermsResponse>(
                command);
    }
}