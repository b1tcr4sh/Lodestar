using NLog.Config;
using NLog.Targets;
using NLog;

namespace Mercurius {
    public static class MCSLogger {
        public static NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public static void Init() {
            LoggingConfiguration config = new LoggingConfiguration();
            Target console = new ConsoleTarget("Systemctl");
            Target logFile = new FileTarget("logFile") { FileName = "latest.log" };

            config.AddRule(LogLevel.Info, LogLevel.Fatal, console);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logFile);

            LogManager.Configuration = config;
        }
    }
}
