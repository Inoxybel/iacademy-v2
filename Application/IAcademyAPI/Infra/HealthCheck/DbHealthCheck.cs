using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace IAcademyAPI.Infra.HealthCheck;

public class DbHealthCheck : IHealthCheck
{
    private readonly IMongoDatabase mongoDatabase;

    public DbHealthCheck(IMongoDatabase mongoDatabase)
    {
        this.mongoDatabase = mongoDatabase;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await mongoDatabase.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
            return HealthCheckResult.Healthy("Database Up And Running.");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Database Up And Running.");
        }
    }
}