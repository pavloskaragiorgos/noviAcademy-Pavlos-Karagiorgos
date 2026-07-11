using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorldRank.Application.Interfaces;
using WorldRank.Infrastructure.Repositories;

namespace WorldRank.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // In-memory repositories hold state, so they must live for the whole app (Singleton).
        services.AddSingleton<IPlayerRepository, InMemoryPlayerRepository>();
        services.AddSingleton<IWalletRepository, InMemoryWalletRepository>();

        services.AddDbContext<WorldRankDbContext>(options =>
        {
            options.UseSqlServer("Server=localhost;Database=WorldRankDb;Integrated Security=true;TrustServerCertificate=true");

        });

        return services;
    }
}
