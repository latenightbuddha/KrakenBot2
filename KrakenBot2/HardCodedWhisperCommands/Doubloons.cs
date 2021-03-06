﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedWhisperCommands
{
    public static class Doubloons
    {
        public static void handleCommand(TwitchLib.TwitchWhisperClient.OnCommandReceivedArgs e)
        {
            string doubloons = WebCalls.getUserDoubloons(e.Username).Result;
            if (Common.WhisperClient != null)
                Common.WhisperClient.SendWhisper(e.Username, string.Format("Your current doubloon count is: {0}", doubloons), Common.DryRun);
        }
    }
}
