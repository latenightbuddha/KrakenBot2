using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    class Games
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                Common.ChatClient.sendMessage(string.Format("!Games is an automated giveaway system! You can see a video on it here: https://www.twitch.tv/burkeblack/v/30553157 .  There are currently {0} !games available!", WebCalls.downloadExGamesCount().Result), Common.DryRun);
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
            if (e.ArgumentsAsList.Count != 0)
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
