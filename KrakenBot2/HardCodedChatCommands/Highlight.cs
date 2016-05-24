using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Highlight
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if(verifyCommand(e))
            {
                if (WebCalls.createHighlight(e.ChatMessage.Username, e.ArgumentsAsString).Result)
                    Common.ChatClient.SendMessage(string.Format("Created a highlight marker by '{0}' titled '{1}'! You can view bookmarked highlights here: https://burkeblack.tv/highlights.php", e.ChatMessage.Username, e.ArgumentsAsString), Common.DryRun);
                else
                    Common.ChatClient.SendMessage("Highlight was not created.  Please make sure Burke is online before creating a highlight.");

                Common.command(e.Command, true);
            } else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            TwitchLib.ChatMessage.UType userType = e.ChatMessage.UserType;
            if (Common.Moderators.Contains(e.ChatMessage.Username.ToLower()))
                userType = TwitchLib.ChatMessage.UType.Moderator;
            if (!Common.Cooldown.chatCommandAvailable(userType, e.Command, 10))
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
