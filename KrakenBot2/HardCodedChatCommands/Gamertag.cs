using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Gamertag
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if(verifyCommand(e) && !Common.DryRun)
            {
                Common.ChatClient.sendMessage("This command is not currently supported");
                /* XboxLeaders_API.Query xQuery = new XboxLeaders_API.Query(e.ArgumentsAsString);
                if (xQuery.Exists)
                    if (xQuery.RecentGames.Count != 0)
                        Common.ChatClient.sendMessage(string.Format("{0} has {1} gamerscore and most recently played {2} where they've accumulated {3} ({4} GS) of the available {5} ({6} GS) achievements!", 
                            xQuery.Gamertag, xQuery.Gamerscore, xQuery.RecentGames[0].Title, xQuery.RecentGames[0].EarnedAchievements, xQuery.RecentGames[0].EarnedGamerscore, xQuery.RecentGames[0].TotalAchievements,
                            xQuery.RecentGames[0].TotalGamerscore));
                    else
                        Common.ChatClient.sendMessage(string.Format("{0} has {1} gamerscore and hasn't played (or their profile is privated) any games recently.", xQuery.Gamertag, xQuery.Gamerscore));
                else
                    Common.ChatClient.sendMessage(string.Format("The gamertag '{0}' is invalid/not found.", e.ArgumentsAsString));
                */
                Common.command(e.Command, true);
            } else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            TwitchLib.ChatMessage.uType userType = e.ChatMessage.UserType;
            if (Common.Moderators.Contains(e.ChatMessage.Username.ToLower()))
                userType = TwitchLib.ChatMessage.uType.Moderator;
            if (!Common.Cooldown.chatCommandAvailable(userType, e.Command, 10))
                return false;
            if (e.ArgumentsAsList.Count == 0)
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
