using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Title
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                TwitchLib.TwitchApi.UpdateStreamTitle(e.ArgumentsAsString, "burkeblack", Properties.Settings.Default.BurkeOAuth);
                Common.ChatClient.SendMessage("Title change API request sent!", Common.DryRun);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (!Common.Cooldown.chatCommandAvailable(e.ChatMessage.UserType, e.Command, 0))
                return false;
            if (!Common.isMod(e))
                return false;
            if (e.ArgumentsAsList.Count == 0)
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
