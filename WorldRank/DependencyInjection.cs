using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

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

            return services;
        }
    }
}
