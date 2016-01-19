using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedWhisperCommands.OnlineNotifications
{
    public class NotifyMe
    {
        public static void handleCommand(TwitchLib.TwitchWhisperClient.CommandReceivedArgs e)
        {
            Common.OnlineNotifier.notifyMe(e.Username);
            Common.command(e.Command, true);
        }
    }
}
