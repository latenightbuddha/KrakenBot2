using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands.OnlineNotifications
{
    public static class RemoveMe
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            Common.OnlineNotifier.removeMe(e.ChatMessage.Username);
            Common.command(e.Command, true);
        }
    }
}
