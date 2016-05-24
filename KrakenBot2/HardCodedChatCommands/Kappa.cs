using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Kappa
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                Common.ChatClient.SendMessage("Kappa", Common.DryRun);
                Common.command(e.Command, true);
            }
            else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (e.ChatMessage.Username.ToLower() != "swiftyspiffy")
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
