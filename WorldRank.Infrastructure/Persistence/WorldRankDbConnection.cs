namespace WorldRank.Infrastructure.Persistence;

// Single place for the default connection string — used by the design-time migration
// factory and as AddInfrastructure's fallback when no connection string is supplied.
public static class WorldRankDbConnection
{
    public const string DefaultConnectionString =
        "Server=localhost;Database=WorldRank;Integrated Security=true;TrustServerCertificate=true";
}