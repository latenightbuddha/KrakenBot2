﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Raid
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                //if (Common.RaidClient == null || Common.RaidClient.Channel.ToLower() != e.ArgumentsAsList[0].ToLower())
                //    launchRaidInstance(e.ArgumentsAsList[0]);
                Common.ChatClient.SendMessage(string.Format("Go raid {0}!!! Viewers: R)", e.ArgumentsAsList[0]), Common.DryRun);
                Common.ChatClient.SendMessage(string.Format("Go raid http://twitch.tv/{0}!!! Sub: burkeShip burkeFire burkeFire", e.ArgumentsAsList[0]), Common.DryRun);
                Common.ChatClient.SendMessage(string.Format("Go raid {0}!!! Viewers: R)", e.ArgumentsAsList[0]), Common.DryRun);
                Common.ChatClient.SendMessage(string.Format("Go raid http://twitch.tv/{0}!!! Sub: burkeShip burkeFire burkeFire", e.ArgumentsAsList[0]), Common.DryRun);
            }
        }

        private static void launchRaidInstance(string channel)
        {
            Common.RaidInstance = new RaidInstance(channel);
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (!Common.Cooldown.chatCommandAvailable(e.ChatMessage.UserType, e.Command, 0))
                return false;
            if (!Common.isMod(e))
                return false;
            if (e.ArgumentsAsList.Count != 1)
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
