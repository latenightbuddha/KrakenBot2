﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Mods
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if(verifyCommand(e))
            {
                Common.ChatClient.sendMessage(getModerators(e.Channel), Common.DryRun);
                Common.command(e.Command, true);
            } else
            {
                Common.command(e.Command, false);
            }
        }

        private static string getModerators(string channel)
        {
            List<TwitchLib.Chatter> chatters = TwitchLib.TwitchAPI.getChatters(channel).Result;
            string ret = "";
            foreach(TwitchLib.Chatter chatter in chatters)
            {
                if(chatter.UserType == TwitchLib.Chatter.uType.Moderator)
                {
                    if(ret == "")
                    {
                        ret = chatter.Username;
                    } else
                    {
                        ret = ret + ", " + chatter.Username;
                    }
                }
            }
            return ret;
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (!Common.Cooldown.commandAvailable(e.ChatMessage.UserType, e.Command, 0))
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
