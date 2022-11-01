using System;
using System.Threading.Tasks;
using Mercurius.Profiles;
using Mercurius.Configuration;
using Tmds.DBus;

namespace Mercurius.DBus.Commands {
    public class ListProfilesCommand : BaseCommand {
        public override string Name => "ProfileList";
        public override string Description => "Lists currently loaded profiles.";
        public override string Format => "";
        public override bool TakesArgs { get => false; }
        public override ObjectPath ObjectPath { get => _objectPath; }
        private ObjectPath _objectPath = new ObjectPath("/org/mercurius/command/listprofiles");
        public override Task<DbusResponse> ExecuteAsync(string[] args)
        {
            // Not really sure how to respond with input..... ?
            return Task.FromResult<DbusResponse>(new DbusResponse {
                Code = -2,
                Data = "aghhh",
                Message = "Not implemented yet",
                Type = DataType.Error
            });
        }
    }
}