using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WorldRank.Infrastructure.Persistence;

public class WorldRankDbContextFactory : IDesignTimeDbContextFactory<WorldRankDbContext>
{
    public WorldRankDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<WorldRankDbContext>()
            .UseSqlServer(WorldRankDbConnection.DefaultConnectionString)
            .Options;

        return new WorldRankDbContext(options);
    }
}