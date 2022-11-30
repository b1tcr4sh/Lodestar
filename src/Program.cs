using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using Mercurius.DBus;
using Mercurius.Configuration;

namespace Mercurius {
    public class Program {
        public static async Task Main(string[] args) {
            LogManager.ThrowConfigExceptions = true;
            LogManager.LoadConfiguration("NLog.xml");

            var builder = new HostBuilder()
            .ConfigureAppConfiguration((hostingContext, config) => {
                config.AddEnvironmentVariables();

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            })
            .ConfigureServices((hostContext, services) => {
                services.AddSingleton<IHostedService, DaemonService>();                
            })
            .ConfigureServices((hostContext, services) => {
                services.AddSingleton<IHostedService, DbusHandler>();
            })
            .ConfigureLogging((hostingContext, logging) => {
                logging.AddNLog(hostingContext.Configuration);
            });

            await builder.RunConsoleAsync();
        }
    }
}