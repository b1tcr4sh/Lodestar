using Mercurius;
using Mercurius.Configuration;
using Mercurius.Profiles;
using Mercurius.DBus;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NLog;

 namespace Mercurius {

     public class DaemonService : IHostedService, IDisposable {
         private readonly ILogger logger;
        //  private readonly IOptions<DaemonConfig> _config;
         public DaemonService() {
            logger = LogManager.GetCurrentClassLogger();

            //  _logger = logger;
            //  _config = config;
         }

         public Task StartAsync(CancellationToken cancellationToken) {
             logger.Info("Starting Mercurius Daemon...");

            SettingsManager.Init();
            ProfileManager.InitializeDirectory();
            ProfileManager.LoadAllProfiles();

            return Task.CompletedTask;
         }

         public Task StopAsync(CancellationToken cancellationToken) {
             logger.Info("Stopping daemon.");
             return Task.CompletedTask;
         }

         public void Dispose() {
             logger.Info("Disposing....");

         }
     }
 }