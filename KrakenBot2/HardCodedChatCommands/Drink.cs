using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Drink
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                string drinking = WebCalls.downloadDrinking().Result;
                Common.ChatClient.SendMessage(string.Format("Burke is currently drink {0}.", drinking));
                Common.command(e.Command, true);
            }
            else
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
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
