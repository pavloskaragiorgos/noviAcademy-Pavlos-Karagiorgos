using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorldRank.Application.Interfaces;
using WorldRank.Infrastructure.Caching;
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
        services.AddMemoryCache();
        services.AddSingleton<ICache, MemoryCacheStore>();

        if (useDatabase)
        {
            services.AddDbContext<WorldRankDbContext>(options =>
                options.UseSqlServer(string.IsNullOrWhiteSpace(connectionString)
                    ? WorldRankDbConnection.DefaultConnectionString
                    : connectionString));

            services.AddScoped<IPlayerRepository, DBPlayerRepository>();
            services.AddScoped<IWalletRepository, DBWalletRepository>();
        }
        else
        {
            services.AddSingleton<IPlayerRepository, InMemoryPlayerRepository>();
            services.AddSingleton<IWalletRepository, InMemoryWalletRepository>();
        }

        return services;
    }
}