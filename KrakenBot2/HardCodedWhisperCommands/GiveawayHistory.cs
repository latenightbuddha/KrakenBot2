using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedWhisperCommands
{
    class GiveawayHistory
    {
        public static void handleCommand(TwitchLib.TwitchWhisperClient.OnCommandReceivedArgs e)
        {
            if (Common.WhisperClient != null)
                Common.WhisperClient.SendWhisper(e.Username, string.Format("Your giveaway history can be seen here: http://burkeblack.tv/giveaways/listing.php?name={0}", e.Username));
        }
    }
}
