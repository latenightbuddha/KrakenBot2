using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands.OnlineNotifications
{
    public static class NotifyMe
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            Common.OnlineNotifier.notifyMe(e.ChatMessage.Username);
            Common.command(e.Command, true);
        }
    }
}
