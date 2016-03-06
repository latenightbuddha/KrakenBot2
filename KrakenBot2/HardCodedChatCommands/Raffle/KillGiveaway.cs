using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands.Raffle
{
    public static class KillGiveaway
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (verifyCommand(e) && !Common.DryRun)
            {
                if (Common.Raffle != null && Common.Raffle.raffleIsActive())
                {
                    Common.Raffle.killGiveaway();
                    Common.ChatClient.sendMessage(string.Format("[KillGiveaway] Active giveaway for '{0}' has been killed!", Common.Raffle.RaffleName), Common.DryRun);
                } 
                else
                {
                    Common.ChatClient.sendMessage("[KillGiveaway] No active giveaway to kill!", Common.DryRun);
                }

                Common.command(e.Command, true);
            }
            else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (!Common.Cooldown.chatCommandAvailable(e.ChatMessage.UserType, e.Command, 0))
                return false;
            if (Common.DryRun)
                return false;
            if (!Common.isMod(e))
                return false;
            return true;
        }
    }
}
