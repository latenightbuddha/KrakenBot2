using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedWhisperCommands
{
    public static class Commands
    {
        public static void handleCommand(TwitchLib.TwitchWhisperClient.CommandReceivedArgs e)
        {
            if (Common.WhisperClient != null)
                Common.WhisperClient.sendWhisper(e.Username, "The currently available whisper commands are: !notifyme, !removeme, !doubloons, !discordinvite, !giveawayhistory", Common.DryRun);
        }
    }
}
