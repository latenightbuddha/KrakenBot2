using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Pick
    {
        private static string[] intro = { "You're the choosen one", "Aaaaaaaaand, it's you", "Eenie meenie miney mo, I pick" };
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                Random rand = new Random();
                List<TwitchLib.Chatter> chatters = TwitchLib.TwitchAPI.getChatters("burkeblack").Result;
                if (e.ArgumentsAsList.Count == 0)
                {
                    Common.ChatClient.sendMessage(string.Format("{0} {1}!!", intro[rand.Next(0, intro.Length)], chatters[rand.Next(0, chatters.Count)].Username), Common.DryRun);
                } else
                {
                    switch(e.ArgumentsAsList[0])
                    {
                        case "follower":
                            bool foundFollower = false;
                            int iteration = 0;
                            TwitchLib.Chatter candidate = null;
                            while (foundFollower == false && iteration < 5)
                            {
                                candidate = chatters[rand.Next(0, chatters.Count)];
                                if (TwitchLib.TwitchAPI.userFollowsChannel(candidate.Username, "burkeblack").Result)
                                    foundFollower = true;
                                iteration++;
                            }
                            if(foundFollower)
                                Common.ChatClient.sendMessage(string.Format("{0} {1}!!", intro[rand.Next(0, intro.Length)], candidate.Username), Common.DryRun);
                            else
                                Common.ChatClient.sendMessage(string.Format("{0} {1} (no follower found after 5th iteration)!!", intro[rand.Next(0, intro.Length)], candidate.Username), Common.DryRun);
                            break;
                        case "sub":
                            Common.ChatClient.sendMessage("Not implimented at the moment.", Common.DryRun);
                            break;
                        case "subscriber":
                            Common.ChatClient.sendMessage("Not implimented at the moment.", Common.DryRun);
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

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
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
