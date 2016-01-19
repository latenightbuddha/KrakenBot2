using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2
{
    public static class Variables
    {
        //{user} = valid user provided in the command, must also be present in return
        //{sender} = user that sends teh command
        //{recent_sub_name} = most recent sub name
        //{recent_sub_length} = most recent sub length
        //{stream_game} = game burke is currently playing, according to Twitch API
        //{stream_status} = stream's current status
        //{stream_link} = stream link
        //{viewer_count} = number of current viewers
        //{command_count} = number of commands
        //{rotating_message_count} = number of rotating messages
        //{online_status} = "online" or "offline" depending on stream status
        //{raffle_name} = name of raffle
        //{raffle_donator} = donator of raffle
        //{raffle_author} = mod that submitted giveaway

        public static string USER(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (e.ArgumentsAsList.Count == 1)
                return e.ArgumentsAsList[0];
            else
                return null;
        }

        public static string SENDER(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            return e.ChatMessage.DisplayName;
        }

        public static string RECENT_SUB_NAME()
        {
            if (Common.RecentSub != null)
                return Common.RecentSub.Name;
            else
                return "No recent sub.";
        }

        public static string RECENT_SUB_LENGTH()
        {
            if (Common.RecentSub != null)
                return Common.RecentSub.Months.ToString();
            else
                return "730";
        }

        public static string STREAM_GAME()
        {
            if (Common.StreamRefresher.isOnline())
                return Common.StreamRefresher.Stream.Game;
            else
                return "Stream is not online.";
        }

        public static string STREAM_STATUS()
        {
            if (Common.StreamRefresher.isOnline())
                return Common.StreamRefresher.Stream.Channel.Status;
            else
                return "Stream is not online.";
        }

        public static string STREAM_LINK()
        {
            return "http://twitch.tv/burkeblack";
        }

        public static string VIEWER_COUNT()
        {
            if (Common.StreamRefresher.isOnline())
                return Common.StreamRefresher.Stream.Viewers.ToString();
            else
                return "Unknown";
        }

        public static string COMMAND_COUNT()
        {
            return Common.ChatCommands.Count.ToString();
        }

        public static string ROTATING_MESSAGE_COUNT()
        {
            return Common.MessageRotator.Messages.Count.ToString();
        }

        public static string ONLINE_STATUS()
        {
            if (Common.StreamRefresher.isOnline())
                return "online";
            else
                return "offline";
        }

        public static string RAFFLE_NAME()
        {
            if (Common.Raffle != null)
                return Common.Raffle.RaffleName;
            else
                return "Uhh.. what raffle?";
        }

        public static string RAFFLE_DONATOR()
        {
            if (Common.Raffle != null)
                return Common.Raffle.RaffleDonator;
            else
                return "Uhh.. what donator?";
        }

        public static string RAFFLE_AUTHOR()
        {
            if (Common.Raffle != null)
                return Common.Raffle.RaffleAuthor;
            else
                return "Uhh.. what author?";
        }
    }
}
