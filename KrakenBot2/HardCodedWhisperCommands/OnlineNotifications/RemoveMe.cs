using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedWhisperCommands.OnlineNotifications
{
    public class RemoveMe
    {
        public static void handleCommand(TwitchLib.TwitchWhisperClient.OnCommandReceivedArgs e)
        {
            Common.OnlineNotifier.removeMe(e.Username);
            Common.command(e.Command, true);
        }
    }
}
