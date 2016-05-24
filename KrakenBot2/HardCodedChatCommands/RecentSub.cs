using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class RecentSub
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                if (Common.RecentSub != null)
                    if (Common.RecentSub.Months > 1)
                        Common.ChatClient.SendMessage(string.Format("Most recent sub was from {0} who currently maintains a {1} sub spree!", Common.RecentSub.Name, Common.RecentSub.Months.ToString()));
                    else
                        Common.ChatClient.SendMessage(string.Format("Most recent sub was from {0} who subbed for their first time!", Common.RecentSub.Name));
                else
                    Common.ChatClient.SendMessage("No recent donations recorded :(");
            }

        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            TwitchLib.ChatMessage.UType userType = e.ChatMessage.UserType;
            if (Common.Moderators.Contains(e.ChatMessage.Username.ToLower()))
                userType = TwitchLib.ChatMessage.UType.Moderator;
            if (!Common.Cooldown.chatCommandAvailable(userType, e.Command, 10))
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
