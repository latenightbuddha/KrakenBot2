﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Pick
    {
        private static string[] intro = { "You're the choosen one", "Aaaaaaaaand, it's you", "Eenie meenie miney mo, I pick" };
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                Random rand = new Random();
                if (e.ArgumentsAsList.Count == 0)
                {
                    List<TwitchLib.Chatter> chatters = TwitchLib.TwitchApi.GetChatters("burkeblack").Result;
                    Common.ChatClient.SendMessage(string.Format("{0} {1}!!", intro[rand.Next(0, intro.Length)], chatters[rand.Next(0, chatters.Count)].Username), Common.DryRun);
                } else
                {
                    switch(e.ArgumentsAsList[0])
                    {
                        case "follower":
                            bool foundFollower = false;
                            int iteration = 0;
                            TwitchLib.Chatter candidate = null;
                            List<TwitchLib.Chatter> chatters = TwitchLib.TwitchApi.GetChatters("burkeblack").Result;
                            while (foundFollower == false && iteration < 5)
                            {
                                candidate = chatters[rand.Next(0, chatters.Count)];
                                if (TwitchLib.TwitchApi.UserFollowsChannel(candidate.Username, "burkeblack").Result)
                                    foundFollower = true;
                                iteration++;
                            }
                            if(foundFollower)
                                Common.ChatClient.SendMessage(string.Format("{0} {1}!!", intro[rand.Next(0, intro.Length)], candidate.Username), Common.DryRun);
                            else
                                Common.ChatClient.SendMessage(string.Format("{0} {1} (no follower found after 5th iteration)!!", intro[rand.Next(0, intro.Length)], candidate.Username), Common.DryRun);
                            break;
                        case "sub":
                            if (Common.ChatSubs.Count > 2)
                                Common.ChatClient.SendMessage(string.Format("{0} {1}!!", intro[rand.Next(0, intro.Length)], Common.ChatSubs[rand.Next(0, Common.ChatSubs.Count)]), Common.DryRun);
                            else
                                Common.ChatClient.SendMessage("Fewer than 3 subs exist in the sub chatter list.  Please allow more subs to speak in chat before using this command.", Common.DryRun);
                            break;
                        case "subscriber":
                            if (Common.ChatSubs.Count > 2)
                                Common.ChatClient.SendMessage(string.Format("{0} {1}!!", intro[rand.Next(0, intro.Length)], Common.ChatSubs[rand.Next(0, Common.ChatSubs.Count)]), Common.DryRun);
                            else
                                Common.ChatClient.SendMessage("Fewer than 3 subs exist in the sub chatter list.  Please allow more subs to speak in chat before using this command.", Common.DryRun);
                            break;
                        case "clear":
                            Common.ChatSubs.Clear();
                            Common.ChatClient.SendMessage("Subscriber list has been cleared.");
                            break;
                        case "reset":
                            Common.ChatSubs.Clear();
                            Common.ChatClient.SendMessage("Subscriber list has been cleared.");
                            break;
                    }
                }
                Common.command(e.Command, true);
            }
            else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (!Common.isMod(e))
                return false;
            if (e.ArgumentsAsList.Count > 1)
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
