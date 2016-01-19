using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Highlight
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if(verifyCommand(e))
            {
                if (WebCalls.createHighlight(e.ChatMessage.Username, e.ArgumentsAsString).Result)
                    Common.ChatClient.sendMessage(string.Format("Created a highlight marker by '{0}' titled '{1}'!", e.ChatMessage.Username, e.ArgumentsAsString), Common.DryRun);
                else
                    Common.ChatClient.sendMessage("Highlight was not created.  Please make sure Burke is online before creating a highlight.");

                Common.command(e.Command, true);
            } else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (!Common.Cooldown.commandAvailable(e.ChatMessage.UserType, e.Command, 10))
                return false;
            if (!Common.isSub(e))
                return false;
            if (e.ArgumentsAsList.Count == 0)
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
