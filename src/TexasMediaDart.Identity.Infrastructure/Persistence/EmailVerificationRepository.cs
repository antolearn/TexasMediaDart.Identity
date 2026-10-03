using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TexasMediaDart.Identity.Application.Abstractions.Persistence;
using TexasMediaDart.Identity.Application.Features.Authentication.VerifyEmail;

namespace TexasMediaDart.Identity.Infrastructure.Persistence;

public sealed class EmailVerificationRepository
    : IEmailVerificationRepository
{
    private readonly string _connectionString;

    public EmailVerificationRepository(
        IConfiguration configuration)
    {
        _connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured.");
    }

    public async Task<EmailVerificationResult> VerifyAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        await using var connection =
            new SqlConnection(_connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_EmailVerificationToken_Verify",
            parameters: new
            {
                TokenHash = tokenHash
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        try
        {
            return await connection
                .QuerySingleAsync<EmailVerificationResult>(command);
        }
        catch (SqlException ex) when (ex.Number == 50020)
        {
            throw new EmailVerificationException(
                EmailVerificationFailureReason.InvalidToken,
                "The email verification link is invalid.");
        }
        catch (SqlException ex) when (ex.Number == 50021)
        {
            throw new EmailVerificationException(
                EmailVerificationFailureReason.AlreadyUsed,
                "The email verification link has already been used.");
        }
        catch (SqlException ex) when (ex.Number == 50022)
        {
            throw new EmailVerificationException(
                EmailVerificationFailureReason.InvalidatedToken,
                "The email verification link is no longer valid.");
        }
        catch (SqlException ex) when (ex.Number == 50023)
        {
            throw new EmailVerificationException(
                EmailVerificationFailureReason.Expired,
                "The email verification link has expired.");
        }
        catch (SqlException ex) when (ex.Number == 50024)
        {
            throw new EmailVerificationException(
                EmailVerificationFailureReason.UserUnavailable,
                "The account is unavailable.");
        }
    }
}