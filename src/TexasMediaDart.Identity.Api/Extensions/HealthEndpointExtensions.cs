using System.Reflection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Data.SqlClient;

namespace TexasMediaDart.Identity.Api.Extensions;

public static class HealthEndpointExtensions
{
    public static WebApplication MapHealthEndpoints(
        this WebApplication app,
        string connectionString)
    {
        var databaseName =
            new SqlConnectionStringBuilder(connectionString).InitialCatalog;

        app.MapHealthChecks(
            "/health/db",
            new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";

                    string? databaseVersion = null;

                    try
                    {
                        await using var connection =
                            new SqlConnection(connectionString);

                        await connection.OpenAsync();

                        await using var command =
                            new SqlCommand(
                                """
                                SELECT TOP (1) [Version]
                                FROM dbo.DatabaseVersion
                                WHERE Id = 1;
                                """,
                                connection);

                        var result =
                            await command.ExecuteScalarAsync();

                        if (result != null &&
                            result != DBNull.Value)
                        {
                            databaseVersion =
                                result.ToString();
                        }
                    }
                    catch
                    {
                        // The registered health check determines
                        // database health. Version lookup should not
                        // replace that result.
                    }

                    await context.Response.WriteAsJsonAsync(new
                    {
                        status = report.Status.ToString(),
                        database = databaseName,
                        databaseVersion =
                            databaseVersion ?? "Not deployed",
                        timestampUtc = DateTime.UtcNow
                    });
                }
            });

        app.MapGet("/health/version", (IHostEnvironment environment) =>
        {
            var assembly = Assembly.GetExecutingAssembly();

            var version =
                assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion
                ?? "unknown";

            return Results.Ok(new
            {
                application = "TexasMediaDart.Identity.Api",
                version,
                environment = environment.EnvironmentName,
                timestampUtc = DateTime.UtcNow
            });
        });

        return app;
    }
}