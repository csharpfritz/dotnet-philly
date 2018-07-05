using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_philly.Startup
{
    internal static class ConfigureServices
    {
        internal static IServiceProvider Execute(ServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(config => {
                config.AddConfiguration(configuration.GetSection("Logging"));
                config.AddConsole();
                config.AddDebug();
            });
            services.AddSingleton<IConsole>(PhysicalConsole.Singleton);

            return services.BuildServiceProvider();
        }
    }
}
