using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorldRank.Application.Interfaces;
using WorldRank.Infrastructure.Persistence;
using WorldRank.Infrastructure.Repositories;

namespace WorldRank.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        bool useDatabase = true,
        string? connectionString = null)
    {
        if (useDatabase)
        {
            services.AddDbContext<WorldRankDbContext>(options =>
                options.UseSqlServer(string.IsNullOrWhiteSpace(connectionString)
                    ? WorldRankDbConnection.DefaultConnectionString
                    : connectionString));

            // Scoped to match the DbContext's lifetime — a Singleton could not safely hold it.
            services.AddScoped<IPlayerRepository, DBPlayerRepository>();
            services.AddScoped<IWalletRepository, DBWalletRepository>();
        }
        else
        {
            // In-memory repositories hold state, so they must live for the whole app (Singleton).
            services.AddSingleton<IPlayerRepository, InMemoryPlayerRepository>();
            services.AddSingleton<IWalletRepository, InMemoryWalletRepository>();
        }

        return services;
    }
}