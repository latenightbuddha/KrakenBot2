using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using DiscordSharp;

namespace KrakenBot2
{
    public static class WebCalls
    {
        public async static Task<List<Objects.TimeoutWord>> downloadTimeoutWords()
        {
            List<Objects.TimeoutWord> words = new List<Objects.TimeoutWord>();
            string jsonStr = await download(Properties.Settings.Default.webTimeoutWordsURL);
            foreach(JToken word in JObject.Parse(jsonStr).SelectToken("words"))
            {
                words.Add(new Objects.TimeoutWord(word));
            }
            return words;
        }

        public async static Task<List<Objects.SpoilerWord>> downloadSpoilerWords()
        {
            List<Objects.SpoilerWord> words = new List<Objects.SpoilerWord>();
            string jsonStr = await download(Properties.Settings.Default.webSpoilerWordsURL);
            foreach(JToken word in JObject.Parse(jsonStr).SelectToken("words"))
            {
                words.Add(new Objects.SpoilerWord(word));
            }
            return words;
        }

        public async static Task<List<Objects.ChatCommand>> downloadChatCommands()
        {
            List<Objects.ChatCommand> commands = new List<Objects.ChatCommand>();
            string jsonStr = await download(Properties.Settings.Default.webCommandsURL);
            foreach(JToken command in JObject.Parse(jsonStr).SelectToken("commands"))
            {
                commands.Add(new Objects.ChatCommand(command));
            }
            return commands;
        }

        public async static Task<List<Objects.Quote>> downloadQuotes()
        {
            List<Objects.Quote> quotes = new List<Objects.Quote>();
            string jsonStr = await download(Properties.Settings.Default.webQuotes);
            foreach(JToken quote in JObject.Parse(jsonStr).SelectToken("quotes"))
            {
                quotes.Add(new Objects.Quote(quote));
            }
            return quotes;
        }

        public async static Task<ServerSettings> downloadServerSettings()
        {
            return new ServerSettings(JObject.Parse(await download(Properties.Settings.Default.webSettingsURL)).SelectToken("settings"));
        }

        public async static Task<string> downloadTimedMessages()
        {
            return await download(Properties.Settings.Default.webMessagesURL);
        }

        public async static Task<List<string>> downloadPreviousWinners(string type)
        {
            List<string> previousWinners = new List<string>();
            string jsonStr = await download(string.Format("{0}?type={1}", Properties.Settings.Default.webPreviousWinners, type));
            foreach(JToken prevWinner in JObject.Parse(jsonStr).SelectToken("previous_winners"))
            {
                previousWinners.Add(prevWinner.ToString());
            }
            return previousWinners;
        }

        public async static Task<List<string>> downloadBlockedViewers()
        {
            List<string> blockedViewers = new List<string>();
            string jsonStr = await download(Properties.Settings.Default.webBlockedViewers);
            foreach(JToken blockedViewer in JObject.Parse(jsonStr).SelectToken("blocked_viewers"))
            {
                blockedViewers.Add(blockedViewer.ToString());
            }
            return blockedViewers;
        }

        public async static Task<int> downloadExGamesCount()
        {
            string jsonStr = await download(Properties.Settings.Default.webExGamesCount);
            return int.Parse(JObject.Parse(jsonStr).SelectToken("exgames_count").ToString());
        }

        public async static Task<List<string>> downloadMinuteEntries(int minutes, List<string> entries)
        {
            string entryStr = "";
            foreach (string entry in entries)
            {
                if (entryStr == "")
                    entryStr = entry;
                else
                    entryStr = entryStr + "," + entry;
            }
            List<string> validEntries = new List<string>();
            string jsonStr = await download(string.Format("{0}?minutes={1}&entries={2}", Properties.Settings.Default.webMinuteEntries, minutes, entryStr));
            foreach(JToken validEntry in JObject.Parse(jsonStr).SelectToken("valid_entries"))
            {
                validEntries.Add(validEntries.ToString());
            }
            return validEntries;
        }

        public async static Task<List<string>> downloadDoubloonEntries(int doubloons, List<string> entries)
        {
            string entryStr = "";
            foreach(string entry in entries)
            {
                if (entryStr == "")
                    entryStr = entry;
                else
                    entryStr = entryStr + "," + entry;
            }
            List<string> validEntries = new List<string>();
            string jsonStr = await download(string.Format("{0}?doubloons={1}&entries={2}", Properties.Settings.Default.webDoubloonEntries, doubloons, entryStr));
            foreach(JToken validEntry in JObject.Parse(jsonStr).SelectToken("valid_entries"))
            {
                validEntries.Add(validEntry.ToString());
            }
            return validEntries;
        }

        public async static Task<int> downloadRaffleID()
        {
            return int.Parse(JObject.Parse(await download(Properties.Settings.Default.webRecentRaffleID)).SelectToken("raffle_id").ToString());
        }

        public async static Task<Objects.RaffleWin> addRaffleWinner(string winner, string raffleName, string donator, string entries, string linker, int claimSeconds)
        {
            JToken raffleServerDetails = JObject.Parse(await download(string.Format("{0}?winner={1}&game={2}&donator={3}&linker={4}&claimtime={5}&entries={6}",
                Properties.Settings.Default.webAddRaffleWinner, winner, raffleName, donator, linker, claimSeconds, entries))).SelectToken("raffle_win");
            Objects.RaffleWin raffleWin = new Objects.RaffleWin(raffleServerDetails);
            return raffleWin;
        }

        public async static Task<List<string>> downloadGlobalEmotes()
        {
            if(!File.Exists("subscriber_emotes.json") || DateTime.UtcNow - File.GetLastWriteTimeUtc("subscriber_emotes.json") > TimeSpan.FromDays(3))
                File.WriteAllText("subscriber_emotes.json", await download("https://twitchemotes.com/api_cache/v2/subscriber.json"));
            if (!File.Exists("global_emotes.json") || DateTime.UtcNow - File.GetLastWriteTimeUtc("global_emotes.json") > TimeSpan.FromDays(3))
                File.WriteAllText("global_emotes.json", await download("https://twitchemotes.com/api_cache/v2/global.json"));
            if (!File.Exists("bttv_emotes.json") || DateTime.UtcNow - File.GetLastWriteTimeUtc("bttv_emotes.json") > TimeSpan.FromDays(3))
                File.WriteAllText("bttv_emotes.json", await download("https://api.betterttv.net/2/emotes"));
            string sub_emotes = File.ReadAllText("subscriber_emotes.json");
            string global_emotes = File.ReadAllText("global_emotes.json");
            string bttv_emotes = File.ReadAllText("bttv_emotes.json");
            List<string> emotes = new List<string>();
            //Parse sub emotes
            foreach(JToken sub in JObject.Parse(sub_emotes).SelectToken("channels"))
            {
                foreach(JToken details in sub)
                {
                    foreach(JToken emote in details.SelectToken("emotes"))
                    {
                        emotes.Add(emote.SelectToken("code").ToString());
                    }
                }
            }
            //Parse global emotes
            JToken gEmotes = JObject.Parse(global_emotes).SelectToken("emotes");
            foreach (JToken emote in gEmotes)
            {
                foreach(JToken part in emote)
                {
                    emotes.Add(((JProperty)part.Parent).Name);
                }
            }
            //Parse bettertwitchtv emotes
            JToken bttvEmotes = JObject.Parse(bttv_emotes).SelectToken("emotes");
            foreach(JToken emote in bttvEmotes)
            {
                emotes.Add(emote.SelectToken("code").ToString());
            }
            Common.rep("Emotes loaded into memory: " + emotes.Count);
            return emotes;
        }

        public async static Task<string> getStreamersMostRecentGame(string streamer)
        {
            TwitchLib.TwitchChannel channelData = await TwitchLib.TwitchAPI.getTwitchChannel(streamer);
            return channelData.Game;
        }

        public async static Task<bool> createHighlight(string username, string title)
        {
            if (Common.StreamRefresher.isOnline())
            {
                TimeSpan uptime = TwitchLib.TwitchAPI.getUptime("burkeblack");
                string timeIn = "";
                if (uptime.Days > 0)
                    timeIn = string.Format("{0} days, {1} hours, {2} minutes, {3} seconds", uptime.Days, uptime.Hours, uptime.Minutes, uptime.Seconds);
                else if (uptime.Hours > 0)
                    timeIn = string.Format("{0} hours, {1} minutes, {2} seconds", uptime.Hours, uptime.Minutes, uptime.Seconds);
                else if (uptime.Minutes > 0)
                    timeIn = string.Format("{0} minutes, {1} seconds", uptime.Minutes, uptime.Seconds);
                else
                    timeIn = string.Format("{0} seconds", uptime.Seconds);
                if (!Common.DryRun)
                    await download(string.Format("{0}?name={1}&title={2}&stream_created_at={3}&time_in={4}", Properties.Settings.Default.webCreateHighlight, username, title, Common.StreamRefresher.Stream.CreatedAt, timeIn));
                return true;
            }
            return false;
        }

        public async static Task<string> getSetPersonalCommand(string username, string newData)
        {
            if(newData == null)
            {
                //Fetch current data
                foreach(Objects.PersonalCommand command in Common.PersonalCommands)
                {
                    if (command.Username.ToLower() == username.ToLower())
                        return command.Data;
                }
                string personalCommandData = await download(string.Format("{0}?name={1}&type={2}", Properties.Settings.Default.webPersonalCommand, username, "fetch"));
                Common.PersonalCommands.Add(new Objects.PersonalCommand(username, personalCommandData));
                return personalCommandData;
            } else
            {
                //Set new data
                if (!Common.DryRun)
                    await download(string.Format("{0}?name={1}&type={2}&newData={3}", Properties.Settings.Default.webPersonalCommand, username, "update", newData));
                bool found = false;
                foreach(Objects.PersonalCommand command in Common.PersonalCommands)
                {
                    if(command.Username.ToLower() == username.ToLower())
                    {
                        command.Data = newData;
                        found = true;
                    }
                }
                if(found == false)
                {
                    Common.PersonalCommands.Add(new Objects.PersonalCommand(username, newData));
                }
                return string.Format("[{0}] Your personal command has been updated!", username);
            }
        }

        public async static Task<string> talk(string username, string message)
        {
            string resp = await download(string.Format("{0}?message={1}", Properties.Settings.Default.webTalk, message));
            return string.Format("[{0}] {1}", username, resp);
        }

        public async static Task<List<Host>> downloadMultihostStreamers()
        {
            List<Host> streamers = new List<Host>();
            string jsonStr = await download(Properties.Settings.Default.webMultihostStreamers);
            foreach (JToken streamer in JObject.Parse(jsonStr).SelectToken("streamers"))
            {
                streamers.Add(new Host(streamer.SelectToken("streamer").ToString(), streamer.SelectToken("information").ToString()));
            }
            return streamers;
        }

        public async static Task<List<Host>> getOnlineMultihostStreamers(List<Host> streamers)
        {
            List<Host> onlineStreamers = new List<Host>();
            foreach(Host streamer in streamers)
            {
                try
                {
                    if (await TwitchLib.TwitchAPI.broadcasterOnline(streamer.Streamer))
                        onlineStreamers.Add(streamer);
                } catch (Exception) { }
            }
            return onlineStreamers;
        }

        public async static Task<List<string>> downloadUsersToNotify()
        {
            List<string> usersToNotify = new List<string>();
            string jsonStr = await download(Properties.Settings.Default.webNotifyMeUsers);
            foreach(JToken user in JObject.Parse(jsonStr).SelectToken("users"))
            {
                usersToNotify.Add(user.SelectToken("username").ToString());
            }
            return usersToNotify;
        }

        public async static void addNotifyUser(string username)
        {
            if (!Common.DryRun)
                await download(string.Format("{0}?name={1}", Properties.Settings.Default.webAddNotifyMeUser, username));
        }

        public async static void removeNotifyUser(string username)
        {
            if (!Common.DryRun)
                await download(string.Format("{0}?name={1}", Properties.Settings.Default.webRemoveNotifyMeUser, username));
        }

        public async static void addDoubloons(string username, int amount)
        {
            if (!Common.DryRun)
                await download(string.Format("{0}?name={1}&amount={2}", Properties.Settings.Default.webAddDoubloons, username, amount));
        }

        public async static void addSoundbyteCredits(string username, int amount)
        {
            if (!Common.DryRun)
                await download(string.Format("{0}?name={1}&amount={2}", Properties.Settings.Default.webAddSoundbyteCredits, username, amount));
        }

        public async static void distibuteDoubloons(int amount)
        {
            if (!Common.DryRun)
                await download(string.Format("{0}?amount={1}", Properties.Settings.Default.webDistributeDoubloons, amount));
        }

        public async static void uploadChatMessageCounts(string userMessageCounts)
        {
            if (!Common.DryRun)
                await download(string.Format("{0}?data={1}", Properties.Settings.Default.webAddChatMessages, userMessageCounts));
        }

        public async static Task<List<string>> downloadTopLevelDomains()
        {
            List<string> tlds = new List<string>();
            string cnts = await download("https://data.iana.org/TLD/tlds-alpha-by-domain.txt");
            int i = 0;
            using (StringReader reader = new StringReader(cnts))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    if (i != 0)
                        tlds.Add(line.ToLower());
                    i++;
                }
            }
            return tlds;
        }

        public async static Task<string> getUserDoubloons(string username)
        {
            string cnts = await download(string.Format("{0}?user={1}", Properties.Settings.Default.webDoubloonCount, username));
            return cnts;
        }

        public async static Task<UpdateDetails> downloadUpdateDetails()
        {
            string cnts = await download(string.Format("{0}?current_version={1}", Properties.Settings.Default.webUpdateRequest, Assembly.GetExecutingAssembly().GetName().Version));
            return new UpdateDetails(JObject.Parse(cnts).SelectToken("update_data"));
        }

        public static void downloadFile(string onlineLocation, string fileName)
        {
            new System.Net.WebClient().DownloadFile(onlineLocation, fileName);
        }

        public async static Task<List<Objects.RecentDonation>> downloadRecentDonations()
        {
            List<Objects.RecentDonation> recentDonations = new List<Objects.RecentDonation>();
            string jsonStr = await download(Properties.Settings.Default.webUpdateDonations);
            foreach(JToken donation in JObject.Parse(jsonStr).SelectToken("donations"))
            {
                recentDonations.Add(new Objects.RecentDonation(donation));
            }
            return recentDonations;
        }

        public async static Task<Objects.YoutubeVideo> getBurkesLatestYTVideo()
        {
            string jsonStr = await download(string.Format("https://www.googleapis.com/youtube/v3/search?key={0}&channelId={1}&part=snippet,id&order=date&maxResults=1", Properties.Settings.Default.YoutubeAPIKey, "UCmW5CTVpCWDM55hZwxtpjyw"));
            string videoID = JObject.Parse(jsonStr).SelectToken("items")[0].SelectToken("id").SelectToken("videoId").ToString();
            string videoDescription = JObject.Parse(jsonStr).SelectToken("items")[0].SelectToken("snippet").SelectToken("description").ToString();
            string videoTitle = JObject.Parse(jsonStr).SelectToken("items")[0].SelectToken("snippet").SelectToken("title").ToString();
            jsonStr = await download(string.Format("https://www.googleapis.com/youtube/v3/videos?id={0}&part=statistics&key={1}", videoID, Properties.Settings.Default.YoutubeAPIKey));
            JToken stats = JObject.Parse(jsonStr).SelectToken("items")[0].SelectToken("statistics");
            return new Objects.YoutubeVideo(videoTitle, videoDescription, videoID, int.Parse(stats.SelectToken("viewCount").ToString()), int.Parse(stats.SelectToken("likeCount").ToString()), int.Parse(stats.SelectToken("dislikeCount").ToString()),
                int.Parse(stats.SelectToken("favoriteCount").ToString()), int.Parse(stats.SelectToken("commentCount").ToString()));
        }

        public async static Task<Objects.YoutubeStats> getYoutubeChannelStats(string channel)
        {
            string jsonStr = await download(string.Format("https://www.googleapis.com/youtube/v3/channels?part=statistics&id={0}&key={1}", "UCmW5CTVpCWDM55hZwxtpjyw", Properties.Settings.Default.YoutubeAPIKey));
            return new Objects.YoutubeStats(JObject.Parse(jsonStr).SelectToken("items")[0].SelectToken("statistics"));
        }

        public async static void notifySwifty(string title, string descriptor)
        {
            await download(string.Format("{0}?title={1}&descriptor={2}", Properties.Settings.Default.webNotify, title, descriptor));
        }

        public async static Task<Objects.FollowData> getFollowData(string channel)
        {
            TwitchLib.TwitchChannel twitchChannel = await TwitchLib.TwitchAPI.getTwitchChannel(channel);
            return new Objects.FollowData(twitchChannel);
        }

        public static string createInviteCode()
        {
            string url = Endpoints.BaseAPI + Endpoints.Channels + $"/{Common.DiscordClient.GetChannelByName("general").id}" + Endpoints.Invites;
            try
            {
                var response = WebWrapper.Post(url, DiscordClient.token, "{\"max_age\":120,\"max_uses\":1,\"temporary\":false,\"xkcdpass\":false}");
                return JObject.Parse(response).SelectToken("code").ToString();
            }
            catch (Exception)
            {
                return "Failed!";
            }
        }

        public async static void addInviteCode(string username, string code)
        {
            await download(string.Format("{0}?user={1}&code={2}", Properties.Settings.Default.webAddInvite, username, code));
        }

        public async static Task<string> getExistingDiscordInvite(string username)
        {
            string jsonStr = await download(string.Format("{0}?user={1}",Properties.Settings.Default.webDiscordOldInvite, username));
            JToken json = JObject.Parse(jsonStr).SelectToken("discord_invite");
            if (json.SelectToken("found").ToString() == "True")
                return json.SelectToken("invite").ToString();
            else
                return null;
        }

        private async static Task<string> download(string url, bool utf8 = false)
        {
            Console.WriteLine("QUERYING: " + url);
            System.Net.WebClient wc = new System.Net.WebClient();
            if (utf8)
                wc.Encoding = Encoding.UTF8;
            return await wc.DownloadStringTaskAsync(url);
        }
    }
}
