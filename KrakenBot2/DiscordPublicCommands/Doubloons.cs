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
                    Common.DiscordClient.GetChannel(e.Channel_ID).SendMessage(string.Format("The doubloon count for '{0}' is: {1}", e.CommandsAsList[0], resp));
                else
                    Common.DiscordClient.GetChannel(e.Channel_ID).SendMessage(string.Format("The user '{0}' was not found/invalid user.", e.CommandsAsList[0]));
            } else
            {
                Common.DiscordClient.GetChannel(e.Channel_ID).SendMessage("You must provide the user you wish to get the doubloon count of.  Use !doubloons swiftyspiffy");
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
