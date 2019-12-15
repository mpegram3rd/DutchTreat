using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DutchTreat.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DutchTreat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // When the project is run
            // Create a host.......Build it.Run it
            var host = CreateHostBuilder(args).Build();

            // Everytime you start Web Server, will try to seed db.
            RunSeeding(host);

            host.Run();
        }

        private static void RunSeeding(IHost host)
        {
            // A way to access DI resources outside the running web instance.
            var scopeFactory = host.Services.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetService<DutchSeeder>();
                seeder.Seed();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(SetupConfiguration)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                                       // Startup.cs
                    webBuilder.UseStartup<Startup>();
                });

        private static void SetupConfiguration(HostBuilderContext ctx, IConfigurationBuilder builder)
        {
            builder.Sources.Clear();

            // Configure "config.json" file with optional "false" and reload "true"
            builder.AddJsonFile("config.json", false, true) // Least trustworthy
                   .AddXmlFile("config.xml", true) // mid level trumping
                   .AddEnvironmentVariables(); // most trustworthy (highest trumping)
        }
    }
}
