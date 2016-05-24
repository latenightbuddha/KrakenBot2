using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Time
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                string timeData = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")).ToString();
                Common.ChatClient.SendMessage(string.Format("The current time for Burke is: {0} {1}", timeData.Split(' ')[1], timeData.Split(' ')[2]), Common.DryRun);
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
