using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;

namespace Mercurius {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = new HostBuilder()
            .ConfigureAppConfiguration((hostingContext, config) => {
                config.AddEnvironmentVariables();

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            })
            .ConfigureServices((hostContext, services) => {
                services.AddOptions();
                services.Configure<DaemonConfig>(hostContext.Configuration.GetSection("Daemon"));

                services.AddSingleton<IHostedService, DaemonService>();
            })
            .ConfigureLogging((hostingContext, logging) => {
                logging.AddNLog(hostingContext.Configuration);

                // logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                // logging.AddConsole();
            });

            await builder.RunConsoleAsync();
        }
    }
}