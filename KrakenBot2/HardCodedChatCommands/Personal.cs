using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Personal
    {
        public static async void handleCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                if(e.Command.ToLower() == e.ChatMessage.Username.ToLower() || e.ChatMessage.Username.ToLower() == "swiftyspiffy")
                {
                    if (e.ArgumentsAsList.Count == 0)
                    {
                        Common.ChatClient.SendMessage(await WebCalls.getSetPersonalCommand(e.ChatMessage.Username, null));
                    }
                    else
                    {
                        string resp = await WebCalls.getSetPersonalCommand(e.ChatMessage.Username, e.ArgumentsAsString);
                        Common.ChatClient.SendMessage(resp, Common.DryRun);
                    }
                }
                Common.command(e.Command, true);
            } else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            TwitchLib.ChatMessage.UType userType = e.ChatMessage.UserType;
            if (Common.Moderators.Contains(e.ChatMessage.Username.ToLower()))
                userType = TwitchLib.ChatMessage.UType.Moderator;
            if (!Common.Cooldown.chatCommandAvailable(userType, e.Command, 10))
                return false;
            if (!Common.isSub(e))
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
