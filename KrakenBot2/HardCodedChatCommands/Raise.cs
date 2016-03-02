using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Raise
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                if(e.ArgumentsAsList.Count == 1)
                {
                    //Raise for sub
                    if(Common.RecentSub != null)
                    {
                        Common.ChatClient.sendMessage(string.Format("burkeFlag burkeFlag burkeFlag raise your burkeFlag for {0} burkeFlag burkeFlag burkeFlag", Common.RecentSub.Name), Common.DryRun);
                        Common.ChatClient.sendMessage(string.Format("burkeFlag burkeFlag burkeFlag raise your burkeFlag for {0} burkeFlag burkeFlag burkeFlag", Common.RecentSub.Name), Common.DryRun);
                        Common.ChatClient.sendMessage(string.Format("burkeFlag burkeFlag burkeFlag raise your burkeFlag for {0} burkeFlag burkeFlag burkeFlag", Common.RecentSub.Name), Common.DryRun);
                    } else
                    {
                        Common.ChatClient.sendMessage("No recent sub recorded (since bot was sorted) :(");
                    }
                } else
                {
                    //Raise
                    Common.ChatClient.sendMessage("burkeFlag burkeFlag burkeFlag raise your burkeFlag burkeFlag burkeFlag", Common.DryRun);
                    Common.ChatClient.sendMessage("burkeFlag burkeFlag burkeFlag raise your burkeFlag burkeFlag burkeFlag", Common.DryRun);
                    Common.ChatClient.sendMessage("burkeFlag burkeFlag burkeFlag raise your burkeFlag burkeFlag burkeFlag", Common.DryRun);
                }
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (!Common.Cooldown.chatCommandAvailable(e.ChatMessage.UserType, e.Command, 0))
                return false;
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
