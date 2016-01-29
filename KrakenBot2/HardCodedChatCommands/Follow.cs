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
                    Common.ChatClient.sendMessage(guessChannelName(e.ArgumentsAsList[0].ToLower()), Common.DryRun);
                Common.command(e.Command, true);
            } else
            {
                Common.command(e.Command, false);
            }
        }

        private static string guessChannelName(string invalidChannel)
        {
            List<TwitchLib.TwitchChannel> results = TwitchLib.TwitchAPI.searchChannels(invalidChannel);
            if (results.Count > 0)
            {
                TwitchLib.TwitchChannel bestGuess = results[0];
                foreach (TwitchLib.TwitchChannel result in results)
                    if (result.Followers > bestGuess.Followers)
                        bestGuess = result;
                return string.Format("The channel '{0}' is invalid or has no status.  Perhaps you meant '{1}'? In that case... {2}", invalidChannel, bestGuess.Name, WebCalls.getFollowData(bestGuess.Name).Result.ChatMessage);
            }
            else
            {
                return string.Format("The channel '{0}' is invalid or has no status, and I could not figure out which channel you meant :(", invalidChannel);
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
