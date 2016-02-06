using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Follow
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                if(Common.LastFollow == null || Common.LastFollow.Channel.Name.ToLower() != e.ArgumentsAsList[0].ToLower())
                    Common.LastFollow = WebCalls.getFollowData(e.ArgumentsAsList[0].ToLower()).Result;
                if (Common.LastFollow != null)
                    Common.ChatClient.sendMessage(Common.LastFollow.ChatMessage, Common.DryRun);
                else
                    Common.ChatClient.sendMessage(string.Format("Failed to query '{0}' channel.  You can still follow them though! http://twitch.tv/{0}", e.ArgumentsAsList[0]), Common.DryRun);
                Common.command(e.Command, true);
            } else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (!Common.Cooldown.commandAvailable(e.ChatMessage.UserType, e.Command, 0))
                return false;
            if (!Common.isMod(e))
                return false;
            if (e.ArgumentsAsList.Count != 1)
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
