using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.DiscordPublicCommands
{
    public static class Doubloons
    {
        public static void handleCommand(Objects.DiscordCommand e)
        {
            if(e.CommandsAsList.Count == 1)
            {
                string resp = WebCalls.getUserDoubloons(e.CommandsAsList[0]).Result;
                if (resp != "n/a")
                    Common.DiscordClient.SendMessageToChannel(string.Format("The doubloon count for '{0}' is: {1}", e.CommandsAsList[0], resp), Common.DiscordClient.GetChannelByName(e.Channel));
                else
                    Common.DiscordClient.SendMessageToChannel(string.Format("The user '{0}' was not found/invalid user.", e.CommandsAsList[0]), Common.DiscordClient.GetChannelByName(e.Channel));
            } else
            {
                Common.DiscordClient.SendMessageToChannel("You must provide the user you wish to get the doubloon count of.  Use !doubloons swiftyspiffy", Common.DiscordClient.GetChannelByName(e.Channel));
            }
        }

        private static bool verifyCommand(Objects.DiscordCommand e)
        {
            if (!Common.Cooldown.discordCommandAvailable(e.Command, 10))
                return false;
            return true;
        }
    }
}
