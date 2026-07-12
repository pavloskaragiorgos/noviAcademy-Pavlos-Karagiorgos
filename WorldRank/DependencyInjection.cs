using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using WorldRank.Application;
using WorldRank.Console.Services;
using WorldRank.Infrastructure;

namespace WorldRank.Console
{
    public static class DependencyInjection
    {
        // Composition root 
        public static IServiceCollection AddWorldRank(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog();
            }
            );
            services.AddApplication();
            services.AddInfrastructure();

            // Console presentation layer (menu classes handle I/O, not the Application services).
            services.AddSingleton<PlayerMenu>();
            services.AddSingleton<WalletMenu>();

            return services;
        }
    }
}
