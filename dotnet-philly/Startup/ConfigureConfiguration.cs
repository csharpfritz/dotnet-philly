using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_philly.Startup
{
    internal static class ConfigureConfiguration
    {
        internal static IConfiguration Execute()
        {
            return new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DotNet_Environment")}.json", true, true)
                // ApplicationData results in the following paths:
                // Windows: <UserProfile>/AppData/Roaming
                // Mac:     /Users/<user>/.config
                // Linux:   /home/<user>/.config
                .AddJsonFile($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/dotnet-philly/appsettings.json", true, true)
                .Build();
        }
    }
}
