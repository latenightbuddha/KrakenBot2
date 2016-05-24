using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class DeathCount
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                switch (e.ArgumentsAsList.Count)
                {
                    case 0:
                        Common.ChatClient.SendMessage(string.Format("Burke's current death count is: {0}", Common.DeathCount), Common.DryRun);
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
                    Common.DeathCount++;
                    break;
                case "+":
                    Common.DeathCount++;
                    break;
                case "--":
                    Common.DeathCount--;
                    break;
                case "-":
                    Common.DeathCount--;
                    break;
                default:
                    if (Common.IsNumeric(arg))
                    {
                        Common.DeathCount = int.Parse(arg);
                        Common.ChatClient.SendMessage(string.Format("Burke's death count has been set to {0} death!", arg), Common.DryRun);
                    }
                    else
                    {
                        Common.ChatClient.SendMessage("Unknown argument.  Try ++, +, -, -- or a number.");
                    }
                    break;
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (e.ArgumentsAsList.Count > 0 && !Common.isMod(e))
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
