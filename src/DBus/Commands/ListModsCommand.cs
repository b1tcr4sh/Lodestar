using System;
using System.Threading.Tasks;
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;
using NLog;

namespace Mercurius.DBus.Commands {
    public class ListModsCommand : BaseCommand {
        public override string Name => "ModList";
        public override string Description => "Lists mods in current profile.";
        public override string Format => "";
        public override bool TakesArgs { get => false; }
        public override ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/listmods");
        private ILogger logger;

        public ListModsCommand(ILogger _logger) : base(_logger) {
            logger = _logger;
        }
        public override Task<DbusResponse> ExecuteAsync(string[] args)
        {
            return Task.FromResult<DbusResponse>(new DbusResponse {
                Code = -2,
                Data = "aghhh",
                Message = "Not implemented yet",
                Type = DataType.Error
            });
        }
    }
}