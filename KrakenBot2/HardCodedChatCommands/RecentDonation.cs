using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class RecentDonation
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                if(Common.RecentDonations.Count != 0)
                {
                    string elapsedTime = "";
                    TimeSpan difference = DateTime.Now.Subtract(Common.RecentDonations[0].Date);
                    if (difference.Days > 0)
                        elapsedTime = string.Format("{0} day(s), {1} hour(s), {2} minute(s), {3} seconds", difference.Days, difference.Hours, difference.Minutes, difference.Seconds);
                    else if (difference.Hours > 0)
                        elapsedTime = string.Format("{0} hour(s), {1} minute(s), {2} seconds", difference.Hours, difference.Minutes, difference.Seconds);
                    else if (difference.Minutes > 0)
                        elapsedTime = string.Format("{0} minute(s), {1} seconds", difference.Minutes, difference.Seconds);
                    else if (difference.Seconds > 0)
                        elapsedTime = string.Format("{0} seconds", difference.Seconds);
                    Common.ChatClient.sendMessage(string.Format("Most recent donation was by {0} who donated ${1} {2} ago, with the message '{3}'", Common.RecentDonations[0].Username,
                        Common.RecentDonations[0].Amount, elapsedTime, Common.RecentDonations[0].Message));
                } else
                {
                    Common.ChatClient.sendMessage("No recent donations recorded :(");
                }
            }

        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (!Common.Cooldown.chatCommandAvailable(e.ChatMessage.UserType, e.Command, 10))
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
