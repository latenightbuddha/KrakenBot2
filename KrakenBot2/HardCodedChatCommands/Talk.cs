﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Talk
    {
        public async static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                Common.ChatClient.sendMessage(await WebCalls.talk(e.ChatMessage.Username, e.ArgumentsAsString), Common.DryRun);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (!Common.Cooldown.commandAvailable(e.ChatMessage.UserType, e.Command, 10))
                return false;
            if (!Common.isSub(e))
                return false;
            if (e.ArgumentsAsList.Count == 0)
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
