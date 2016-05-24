using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Uptime
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                TimeSpan uptime = TwitchLib.TwitchApi.GetUptime("burkeblack").Result;
                string msgStr = "";
                if (uptime.Days > 0)
                    msgStr = uptime.Days + " days";
                if (uptime.Hours > 0)
                    if(msgStr == "")
                        msgStr += string.Format("{0} hours", uptime.Hours);
                    else
                        msgStr += string.Format(", {0} hours", uptime.Hours);


                if (uptime.Minutes > 0)
                    if(msgStr == "")
                        msgStr += string.Format("{0} minutes", uptime.Minutes);
                    else
                        msgStr += string.Format(", {0} minutes", uptime.Minutes);
                if (uptime.Seconds > 0)
                    if(msgStr == "")
                        msgStr += string.Format("{0} seconds", uptime.Seconds);
                    else
                        msgStr += string.Format(", {0} seconds", uptime.Seconds);
                Common.ChatClient.SendMessage(string.Format("Current uptime for BurkeBlack is: {0}", msgStr), Common.DryRun);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (!Common.Cooldown.chatCommandAvailable(e.ChatMessage.UserType, e.Command, 10))
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
