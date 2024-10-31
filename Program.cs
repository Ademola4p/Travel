using ExternalApiClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using Travel.AppData;

namespace Travel
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("   Welcome to our travel service!");
            Task.Run(async () =>
                    {
                        try
                        {
                            var host = CreateHostBuilder(args).Build();
                            var travelService = host.Services.GetRequiredService<TravelService>();

                            var ui = new ConsoleUI(travelService);
                            await ui.RunAsync();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Internal Server Error: {ex.Message}");
                        }
                    }).GetAwaiter().GetResult();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                      .AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                // Register HttpClient
                services.AddHttpClient<IAccessTokenManager, AccessTokenManager>();

                // Register services
                services.AddScoped<TravelService>();

                // Add logging
                services.AddLogging();
            });
    }
}
