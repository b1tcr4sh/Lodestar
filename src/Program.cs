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
using Mercurius.API;
using Mercurius.Profiles;

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

            FileTarget logFile = new FileTarget();
            logFile.Layout = "[${date:format=HH\\:MM\\:ss}] ${logger} -> ${message}";
            logFile.CleanupFileName = true;
            logFile.ArchiveFileName = "./latest.log";
            logFile.ArchiveFileKind = FilePathKind.Relative;


            NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Trace);
            // NLog.Config.SimpleConfigurator.ConfigureForTargetLogging(logFile, LogLevel.Trace);

            SettingsManager.Init();

            APIs apis = new APIs();
            apis.Add(new ModrinthAPI(@"https://api.modrinth.com/v2/", new HttpClient()));
            apis.Add(new CurseforgeAPI(@"https://api.curseforge.com/", new HttpClient()));

            var builder = new HostBuilder()
            .ConfigureAppConfiguration((hostingContext, config) => {
                config.AddEnvironmentVariables();

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            })
            .ConfigureServices((hostContext, services) => {
                services.AddSingleton<IHostedService, DbusHandler>(); 
                services.AddSingleton<APIs>(apis);      
                services.AddSingleton<ProfileManager>();
            })
            .ConfigureLogging((hostingContext, logging) => {
                logging.AddNLog(hostingContext.Configuration);
            });

            await builder.RunConsoleAsync();
        }
    }
}