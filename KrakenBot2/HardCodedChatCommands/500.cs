using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class _500
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                int subCount = TwitchLib.TwitchAPI.getSubscriberCount("burkeblack", Properties.Settings.Default.BurkeOAuth);
                string message = "The Captain is on a hunt for 500 crew members (Subs) that will lead to full time twitch Piracy, which will also unlock more Booty for the crew (Emotes). He currently has a crew of " + subCount.ToString() + "! So if you love running into trees, poles, bridges, space rocks and wish to join our truly close and caring community, please join! :)";
                Common.ChatClient.sendMessage(message);
                Common.command(e.Command, true);
            }
            else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (!Common.Cooldown.chatCommandAvailable(e.ChatMessage.UserType, e.Command, 10))
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
