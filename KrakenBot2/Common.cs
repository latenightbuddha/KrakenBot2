using System;
using System.Collections.Generic;
using DiscordSharp;

namespace KrakenBot2
{
    public static class Common
    {
        private static string defaultChannel = "burkeblack";
        private static bool defaultOverride = false;
        private static bool dryRun = false;
        private static bool entryMessage = true;
        private static int discordInviteLimit = 500;

        private static TwitchLib.Subscriber recentSub;
        private static List<string> topLevelDomains;
        private static Objects.FollowData lastFollow;

        private static DiscordClient discordClient;
        private static RaidInstance raidInstance;
        private static DonationUpdater donationUpdater;
        private static List<Objects.RecentDonation> recentDonations;
        private static UpdateData updateData;
        private static TwitchLib.TwitchChatClient raidClient;
        private static TwitchLib.TwitchChatClient chatClient;
        private static TwitchLib.TwitchWhisperClient whisperClient;
        private static List<string> cachedEmotes;
        private static List<Objects.TimeoutWord> toWords;
        private static List<Objects.SpoilerWord> spoilerWords;
        private static List<Objects.ChatCommand> chatCommands;
        private static MessageRotator messageRotator;
        private static ServerSettings settings;
        private static Cooldown cooldown = new Cooldown();
        private static Raffle raffle;
        private static List<Objects.Quote> quotes;
        private static List<Objects.PersonalCommand> personalCommands = new List<Objects.PersonalCommand>();
        private static Multihost multihost;
        private static OnlineNotifications onlineNotifier;
        private static PanelCommands panelCommands;
        private static DoubloonDistributor doubloonDistributor;
        private static FollowerTracker followerTracker;
        private static ChatMessageTracker chatMessageTracker;
        private static OpenWeatherAPI.OpenWeatherAPI openWeatherAPI;
        private static StreamRefresher streamRefresher;
        private static List<Objects.Permit> permits = new List<Objects.Permit>();
        private static List<string> moderators = new List<string> { "acebravo69", "andyl_sandbox", "ara_gaming", "arykitty", "batty0", "bikeman", "bioadmiralx", "bruisedlee01", "burkeblack",
        "burke_listener", "caimspact", "captain_zyloh", "chrizzzzzzz", "clamtaco", "cohhcarnage", "cohhilitionteambot", "cruisette", "dara226", "dianadd", "djtechlive", "docgotgame",
        "ebrech", "echoics", "ellohime", "evilbunny101", "ferretbomb", "fu_grimreaper1979", "fuzzyfreaks", "g0dispink", "geekdomo", "itsgime", "jazzalynn", "jonsmith13", "kingradinov",
        "knightfire5513", "larianstudios", "limpy", "lowco2525", "moblord", "molly", "mrboombati", "northernhermit", "omeed", "oodigames", "radlit1", "rallei_lol", "sattelizergames",
        "skylatron", "sumsum5513", "swiftyspiffy", "tangentgaming", "tbcquartermasterbot", "tessachka", "the_kraken_bot", "tobes101", "totalwarofficial", "tr0ysefix", "undeadrme",
        "warwitchtv", "wolfsgorawr", "xorshasia", "xwarcrimesx" };

        public static List<string> TopLevelDomains { get { return topLevelDomains; } set { topLevelDomains = value; } }
        public static List<Objects.RecentDonation> RecentDonations { get { return recentDonations; } set { recentDonations = value; } }
        public static Objects.FollowData LastFollow { get { return lastFollow; } set { lastFollow = value; } }

        public static int DiscordInviteLimit { get { return discordInviteLimit; } }
        public static DiscordClient DiscordClient { get { return discordClient; } set { discordClient = value; } }
        public static RaidInstance RaidInstance { get { return raidInstance; } set { raidInstance = value; } }
        public static DonationUpdater DonationUpdater { get { return donationUpdater; } set { donationUpdater = value; } }
        public static UpdateData UpdateDatas { get { return updateData; } set { updateData = value; } }
        public static string DefaultChannel { get { return defaultChannel; } set { defaultChannel = value; } }
        public static bool DefaultOverride { get { return defaultOverride; } }
        public static bool DryRun { get { return dryRun; } }
        public static bool EntryMessage { get { return entryMessage; } }
        public static TwitchLib.TwitchChatClient RaidClient { get { return raidClient; } set { raidClient = value; } }
        public static TwitchLib.TwitchChatClient ChatClient { get { return chatClient; } set { chatClient = value; } }
        public static TwitchLib.TwitchWhisperClient WhisperClient { get { return whisperClient; } set { whisperClient = value; } }
        public static List<string> CachedEmotes { get { return cachedEmotes; } set { cachedEmotes = value; } }
        public static List<Objects.TimeoutWord> TimeoutWords { get { return toWords; } set { toWords = value; } }
        public static List<Objects.SpoilerWord> SpoilerWords { get { return spoilerWords; } set { spoilerWords = value; } }
        public static List<Objects.ChatCommand> ChatCommands { get { return chatCommands; } set { chatCommands = value; } }
        public static MessageRotator MessageRotator { get { return messageRotator; } set { messageRotator = value; } }
        public static ServerSettings Settings { get { return settings; } set { settings = value; } }
        public static Cooldown Cooldown { get { return cooldown; } set { cooldown = value; } }
        public static Raffle Raffle { get { return raffle; } set { raffle = value; } }
        public static List<Objects.Quote> Quotes { get { return quotes; } set { quotes = value; } }
        public static List<Objects.PersonalCommand> PersonalCommands { get { return personalCommands; } }
        public static TwitchLib.Subscriber RecentSub { get { return recentSub; } set { recentSub = value; } }
        public static Multihost Multihost { get { return multihost; } set { multihost = value; } }
        public static OnlineNotifications OnlineNotifier { get { return onlineNotifier;  } set { onlineNotifier = value; } }
        public static PanelCommands PanelCommands { get { return panelCommands; } set { panelCommands = value; } }
        public static DoubloonDistributor DoubloonDistributor { get { return doubloonDistributor; } set { doubloonDistributor = value; } }
        public static FollowerTracker FollowerTracker { get { return followerTracker; }set { followerTracker = value; } }
        public static ChatMessageTracker ChatMessageTracker { get { return chatMessageTracker; } set { chatMessageTracker = value; } }
        public static OpenWeatherAPI.OpenWeatherAPI OpenWeatherAPI { get { return openWeatherAPI; } set { openWeatherAPI = value; } }
        public static StreamRefresher StreamRefresher { get { return streamRefresher; } set { streamRefresher = value; } }
        public static List<Objects.Permit> Permits { get { return permits; } set { permits = value; } }
        public static List<string> Moderators { get { return moderators; } set { moderators = value; } }

        public enum GiveawayTypes
        {
            EXGAMES,
            STEAMTRADE,
            STEAMCODE,
            STEAMGIFT,
            ORIGINCODE,
            HUMBLEBUNDLE,
            SERIALCODE,
            LOGITECH,
            SOUND_BYTES,
            OTHER
        }

        public static void command(string command, bool success)
        {
            baseMessage(ConsoleColor.Cyan, "Command: " + command + ", success: " + success);
        }

        public static void success(string data)
        {
            baseMessage(ConsoleColor.Green, data);
        }

        public static void fail(string data)
        {
            baseMessage(ConsoleColor.Red, data);
        }

        public static void notify(string title, string descriptor)
        {
            WebCalls.notifySwifty(title, descriptor);
        }

        private static int color = 0;
        public static void rep(string data)
        {
            switch(color)
            {
                case 0:
                    baseMessage(ConsoleColor.Red, data);
                    color++;
                    break;
                case 1:
                    baseMessage(ConsoleColor.White, data);
                    color++;
                    break;
                case 2:
                    baseMessage(ConsoleColor.Cyan, data);
                    color = 0;
                    break;
            }
            
        }

        private static void baseMessage(ConsoleColor color, string message)
        {
            Console.ForegroundColor = color;
            Console.Write("[" + DateTime.Now + "]");
            Console.WriteLine(message);
        }

        public static bool isSub(TwitchLib.TwitchChatClient.NewChatMessageArgs e)
        {
            TwitchLib.ChatMessage.uType uType = e.ChatMessage.UserType;
            if (uType == TwitchLib.ChatMessage.uType.Admin || uType == TwitchLib.ChatMessage.uType.GlobalModerator || uType == TwitchLib.ChatMessage.uType.Moderator ||
                uType == TwitchLib.ChatMessage.uType.Staff || e.ChatMessage.Subscriber || Moderators.Contains(e.ChatMessage.Username.ToLower()) || 
                e.ChatMessage.Username.ToLower() == "swiftyspiffy")
                return true;
            else
                return false;
        }

        public static bool isSub(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            TwitchLib.ChatMessage.uType uType = e.ChatMessage.UserType;
            if (uType == TwitchLib.ChatMessage.uType.Admin || uType == TwitchLib.ChatMessage.uType.GlobalModerator || uType == TwitchLib.ChatMessage.uType.Moderator ||
                uType == TwitchLib.ChatMessage.uType.Staff || e.ChatMessage.Subscriber || Moderators.Contains(e.ChatMessage.Username.ToLower()) ||
                 e.ChatMessage.Username.ToLower() == "swiftyspiffy")
                return true;
            else
                return false;
        }

        public static bool isMod(TwitchLib.TwitchChatClient.NewChatMessageArgs e)
        {
            TwitchLib.ChatMessage.uType uType = e.ChatMessage.UserType;
            if (uType == TwitchLib.ChatMessage.uType.Admin || uType == TwitchLib.ChatMessage.uType.GlobalModerator || uType == TwitchLib.ChatMessage.uType.Moderator ||
                uType == TwitchLib.ChatMessage.uType.Staff || Moderators.Contains(e.ChatMessage.Username.ToLower()) ||
                 e.ChatMessage.Username.ToLower() == "swiftyspiffy")
                return true;
            else
                return false;
        }

        public static bool isMod(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            TwitchLib.ChatMessage.uType uType = e.ChatMessage.UserType;
            if (uType == TwitchLib.ChatMessage.uType.Admin || uType == TwitchLib.ChatMessage.uType.GlobalModerator || uType == TwitchLib.ChatMessage.uType.Moderator ||
                uType == TwitchLib.ChatMessage.uType.Staff || Moderators.Contains(e.ChatMessage.Username.ToLower()) ||
                 e.ChatMessage.Username.ToLower() == "swiftyspiffy")
                return true;
            else
                return false;
        }

        public static bool IsNumeric(string value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value, "^\\d+$");
        }

        public class UpdateData
        {
            private UpdateDetails details;
            
            public UpdateDetails Details { get { return details; } }

            public UpdateData(UpdateDetails details)
            {
                this.details = details;
            }
        }
    }
}
