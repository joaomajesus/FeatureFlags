using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;

namespace TestFeatureFlags.Api1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var connectionString = config.Build().GetConnectionString("AppConfig");

                    if (connectionString != null)
                        config.AddAzureAppConfiguration(options =>
                        {
                            options.Connect(connectionString)
                                   .ConfigureRefresh(refresh =>
                                   {
                                       refresh.Register("TestFeatureFlags-Api1:Settings:Sentinel", refreshAll: true)
                                              .SetCacheExpiration(new TimeSpan(0, 0, 10));
                                   })
                                   .Select(KeyFilter.Any)
                                   .Select(KeyFilter.Any, hostingContext.HostingEnvironment.EnvironmentName)
                                   .UseFeatureFlags();
                        });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    
}
