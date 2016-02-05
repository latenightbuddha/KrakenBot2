using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class HitCount
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                switch(e.ArgumentsAsList.Count)
                {
                    case 0:
                        Common.ChatClient.sendMessage(string.Format("Burke's current hit count is: {0}", Common.HitCount), Common.DryRun);
                        break;
                    case 1:
                        handleUpdatingArg(e.ArgumentsAsList[0]);
                        break;
                }
                Common.command(e.Command, true);
            }
            else
            {
                Common.command(e.Command, false);
            }
        }

        private static void handleUpdatingArg(string arg)
        {
            switch (arg)
            {
                case "++":
                    Common.HitCount++;
                    break;
                case "+":
                    Common.HitCount++;
                    break;
                case "--":
                    Common.HitCount--;
                    break;
                case "-":
                    Common.HitCount--;
                    break;
                default:
                    if(Common.IsNumeric(arg))
                    {
                        Common.HitCount = int.Parse(arg);
                        Common.ChatClient.sendMessage(string.Format("Burke's hit count has been set to {0} hits!", arg), Common.DryRun);
                    } else
                    {
                        Common.ChatClient.sendMessage("Unknown argument.  Try ++, +, -, -- or a number.");
                    }
                    break;
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (e.ArgumentsAsList.Count > 0 && !Common.isMod(e))
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
