using System;

namespace Mercurius {
    public class CommandHandler {
        private string[] Args;
        private string Command;
        public CommandHandler(string[] args) {
            if (args is null) throw new ArgumentNullException("args was null");
            
            Args = ParseArgs(args);
        }

        public ExecuteCommand() {
            
        }

        private string[] ParseArgs(string[] args) {
            Command = args[0];
            
            for (int i = 0; i < args.Length; i++) {
                if (i == 0) args[0] = args[1];
                    else args[i] = args[i - 1];
            }
            return args;
        }
    }
}