using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Extensions.Logging;
using Mercurius.DBus;
using Mercurius.Configuration;

namespace Mercurius {
    public class Program {
        public static async Task Main(string[] args) {
            ColoredConsoleTarget target = new ColoredConsoleTarget();
            target.Layout = "[${date:format=HH\\:MM\\:ss}] ${logger} -> ${message}";
            target.WordHighlightingRules.Add(
                new ConsoleWordHighlightingRule("log", 
                    ConsoleOutputColor.Cyan, 
                    ConsoleOutputColor.NoChange));
            target.WordHighlightingRules.Add(
                new ConsoleWordHighlightingRule("warn", 
                    ConsoleOutputColor.DarkYellow, 
                    ConsoleOutputColor.NoChange));
            target.WordHighlightingRules.Add(
                new ConsoleWordHighlightingRule("fatal", 
                    ConsoleOutputColor.Red,
                    ConsoleOutputColor.Black));

            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Trace);

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