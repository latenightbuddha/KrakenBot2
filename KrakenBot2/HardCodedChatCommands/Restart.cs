using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public class Restart
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                Common.ChatClient.SendMessage("Restarting...", Common.DryRun);
                System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\KrakenBot2.exe");
                Environment.Exit(0);
                Common.command(e.Command, true);
            }
            else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (!Common.Cooldown.chatCommandAvailable(e.ChatMessage.UserType, e.Command, 0))
                return false;
            if (!Common.isMod(e))
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
