using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands.Raffle
{
    public static class Claim
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if(verifyCommand(e) && !Common.DryRun)
            {
                Common.Raffle.tryClaim(e.ChatMessage.Username);
                Common.command(e.Command, true);
            } else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (!Common.Cooldown.chatCommandAvailable(e.ChatMessage.UserType, e.Command, 0))
                return false;
            if (Common.Raffle == null || !Common.Raffle.raffleIsActive())
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
