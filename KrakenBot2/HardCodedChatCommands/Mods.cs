using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Mods
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if(verifyCommand(e))
            {
                Common.ChatClient.SendMessage(getModerators(e.Channel), Common.DryRun);
                Common.command(e.Command, true);
            } else
            {
                Common.command(e.Command, false);
            }
        }

        private static string getModerators(string channel)
        {
            List<TwitchLib.Chatter> chatters = TwitchLib.TwitchApi.GetChatters(channel).Result;
            string ret = "";
            foreach(TwitchLib.Chatter chatter in chatters)
            {
                if(chatter.UserType == TwitchLib.Chatter.UType.Moderator)
                {
                    if(ret == "")
                    {
                        ret = chatter.Username;
                    } else
                    {
                        ret = ret + ", " + chatter.Username;
                    }
                }
            }
            return ret;
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
