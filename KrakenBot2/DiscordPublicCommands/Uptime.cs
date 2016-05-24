using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.DiscordPublicCommands
{
    public class Uptime
    {
        public static void handleCommand(Objects.DiscordCommand e)
        {
            if (verifyCommand(e))
            {
                if(Common.StreamRefresher.isOnline())
                {
                    TimeSpan uptime = TwitchLib.TwitchApi.GetUptime("burkeblack").Result;
                    string msgStr = "";
                    if (uptime.Days > 0)
                        msgStr = uptime.Days + " days";
                    if (uptime.Hours > 0)
                        if (msgStr == "")
                            msgStr += string.Format("{0} hours", uptime.Hours);
                        else
                            msgStr += string.Format(", {0} hours", uptime.Hours);

                    if (uptime.Minutes > 0)
                        if (msgStr == "")
                            msgStr += string.Format("{0} minutes", uptime.Minutes);
                        else
                            msgStr += string.Format(", {0} minutes", uptime.Minutes);
                    if (uptime.Seconds > 0)
                        if (msgStr == "")
                            msgStr += string.Format("{0} seconds", uptime.Seconds);
                        else
                            msgStr += string.Format(", {0} seconds", uptime.Seconds);
                    Common.DiscordClient.GetChannel(e.Channel_ID).SendMessage("BurkeBlack's current uptime is: " + msgStr);
                } else
                {
                    Common.DiscordClient.GetChannel(e.Channel_ID).SendMessage("BurkeBlack does not appear to be online.");
                }
                Common.command(e.Command, true);
            }
            else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(Objects.DiscordCommand e)
        {
            if (!Common.Cooldown.discordCommandAvailable(e.Command, 120))
                return false;
            return true;
        }
    }
}
