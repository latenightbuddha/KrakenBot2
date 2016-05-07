using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Weather
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                OpenWeatherAPI.Query query = Common.OpenWeatherAPI.query(e.ArgumentsAsString);
                if (query != null && query.ValidRequest)
                    Common.ChatClient.sendMessage(string.Format("{0}, {1} temperature: {2} ~F ({3} ~C).Conditions: {4} ({5}). Humidity: {6}%. Wind speed: {7} m/s ({8})",
                        query.Name, query.Sys.Country, query.Main.Temperature.FahrenheitCurrent, query.Main.Temperature.CelsiusCurrent, query.Weathers[0].Main, query.Weathers[0].Description,
                        query.Main.Humdity, query.Wind.SpeedMetersPerSecond, query.Wind.directionEnumToString(query.Wind.Direction)), Common.DryRun);
                else
                    Common.ChatClient.sendMessage("Failed to process weather command. Sorry :(", Common.DryRun);
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
