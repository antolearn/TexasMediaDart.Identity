using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace TexasMediaDart.Identity.Api.BackgroundServices;

public sealed class RefreshTokenCleanupService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RefreshTokenCleanupService> _logger;

    public RefreshTokenCleanupService(
        IConfiguration configuration,
        ILogger<RefreshTokenCleanupService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var intervalHours =
            int.TryParse(
                _configuration["RefreshTokenCleanup:IntervalHours"],
                out var parsedHours)
                ? parsedHours
                : 24;

        var retentionDays =
            int.TryParse(
                _configuration["RefreshTokenCleanup:RetentionDays"],
                out var parsedDays)
                ? parsedDays
                : 30;

        _logger.LogInformation(
            "Refresh token cleanup service started. IntervalHours={IntervalHours}, RetentionDays={RetentionDays}",
            intervalHours,
            retentionDays);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupAsync(
                    retentionDays,
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Refresh token cleanup failed.");
            }

            await Task.Delay(
                TimeSpan.FromHours(intervalHours),
                stoppingToken);
        }
    }

    private async Task CleanupAsync(
        int retentionDays,
        CancellationToken cancellationToken)
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogWarning(
                "Refresh token cleanup skipped because DefaultConnection is not configured.");

            return;
        }

        await using var connection =
            new SqlConnection(connectionString);

        var command = new CommandDefinition(
            commandText: "dbo.sp_RefreshTokens_Cleanup",
            parameters: new
            {
                RetentionDays = retentionDays
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var result =
        await connection.QuerySingleAsync<CleanupResult>(command);

    _logger.LogInformation(
        "Refresh token cleanup completed. DeletedCount={DeletedCount}",
        result.DeletedCount);
    }
    private sealed class CleanupResult
    {
        public int DeletedCount { get; set; }
    }

}

