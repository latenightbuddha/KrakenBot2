using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using DiscordSharp;
using System.Net.Http;
using System.Net;

namespace KrakenBot2
{
    public static class WebCalls
    {
        // Downloads and loads timeout words into list; returns list
        public async static Task<List<Objects.TimeoutWord>> downloadTimeoutWords()
        {
            List<Objects.TimeoutWord> words = new List<Objects.TimeoutWord>();
            string jsonStr = await request(requestType.GET, Properties.Settings.Default.webTimeoutWordsURL);
            foreach(JToken word in JObject.Parse(jsonStr).SelectToken("words"))
                words.Add(new Objects.TimeoutWord(word));
            return words;
        }

        // Downloads and loads spoiler words into list; returns list
        public async static Task<List<Objects.SpoilerWord>> downloadSpoilerWords()
        {
            List<Objects.SpoilerWord> words = new List<Objects.SpoilerWord>();
            string jsonStr = await request(requestType.GET, Properties.Settings.Default.webSpoilerWordsURL);
            foreach(JToken word in JObject.Parse(jsonStr).SelectToken("words"))
                words.Add(new Objects.SpoilerWord(word));
            return words;
        }

        // Downloads and loads chat commands into list; returns list
        public async static Task<List<Objects.ChatCommand>> downloadChatCommands()
        {
            List<Objects.ChatCommand> commands = new List<Objects.ChatCommand>();
            string jsonStr = await request(requestType.GET, Properties.Settings.Default.webCommandsURL);
            foreach(JToken command in JObject.Parse(jsonStr).SelectToken("commands"))
                commands.Add(new Objects.ChatCommand(command));
            return commands;
        }

        // Download and loads quotes into list; returns list
        public async static Task<List<Objects.Quote>> downloadQuotes()
        {
            List<Objects.Quote> quotes = new List<Objects.Quote>();
            string jsonStr = await request(requestType.GET, Properties.Settings.Default.webQuotes);
            foreach(JToken quote in JObject.Parse(jsonStr).SelectToken("quotes"))
                quotes.Add(new Objects.Quote(quote));
            return quotes;
        }

        // Downloads server settings; returns them in form of ServerSetting object
        public async static Task<ServerSettings> downloadServerSettings()
        {
            return new ServerSettings(JObject.Parse(await request(requestType.GET, Properties.Settings.Default.webSettingsURL)).SelectToken("settings"));
        }

        // Downloads and returns JSON string of timed messages
        public async static Task<string> downloadTimedMessages()
        {
            return await request(requestType.GET, Properties.Settings.Default.webMessagesURL);
        }

        // Downloads and loads previous raffle winners into list; returns list
        public async static Task<List<Objects.PreviousRaffleWinner>> downloadPreviousWinners()
        {
            List<Objects.PreviousRaffleWinner> previousWinners = new List<Objects.PreviousRaffleWinner>();
            string jsonStr = await request(requestType.GET, Properties.Settings.Default.webPreviousWinners);
            foreach(JToken prevWinner in JObject.Parse(jsonStr).SelectToken("previous_winners"))
                previousWinners.Add(new Objects.PreviousRaffleWinner(prevWinner));
            return previousWinners;
        }

        // Downloads and returns JSON string of blocked viewers
        public async static Task<List<string>> downloadBlockedViewers(string donator)
        {
            List<string> blockedViewers = new List<string>();
            string jsonStr = await request(requestType.GET, string.Format("{0}?donator={1}", Properties.Settings.Default.webBlockedViewers, donator));
            foreach(JToken blockedViewer in JObject.Parse(jsonStr).SelectToken("blocked_viewers"))
                blockedViewers.Add(blockedViewer.ToString());
            return blockedViewers;
        }

        // Downloads !Games count and returns the int value
        public async static Task<int> downloadExGamesCount()
        {
            string jsonStr = await request(requestType.GET, Properties.Settings.Default.webExGamesCount);
            return int.Parse(JObject.Parse(jsonStr).SelectToken("exgames_count").ToString());
        }

        // Downloads and loads minute limiting entries into list; returns list
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
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("minutes", minutes.ToString()),
                new KeyValuePair<string, string>("entries",entryStr)
            };
            string jsonStr = await request(requestType.GET, Properties.Settings.Default.webMinuteEntries, args);
            foreach(JToken validEntry in JObject.Parse(jsonStr).SelectToken("valid_entries"))
                validEntries.Add(validEntries.ToString());
            return validEntries;
        }

        // Downloads doubloon limiting users and loads them into list; returns list
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
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("doubloons", doubloons.ToString()),
                new KeyValuePair<string, string>("entries", entryStr)
            };
            string jsonStr = await request(requestType.GET, Properties.Settings.Default.webDoubloonEntries, args);
            foreach(JToken validEntry in JObject.Parse(jsonStr).SelectToken("valid_entries"))
                validEntries.Add(validEntry.ToString());
            return validEntries;
        }

        // Downloads raffle ID of the most recently successfully run giveaway, and return int value
        public async static Task<int> downloadRaffleID()
        {
            return int.Parse(JObject.Parse(await request(requestType.GET, Properties.Settings.Default.webRecentRaffleID)).SelectToken("raffle_id").ToString());
        }

        // Performs a GET request with giveaway details to BurkeBlack.TV 
        public async static Task<Objects.RaffleWin> addRaffleWinner(string winner, string raffleName, string donator, string entries, string linker, int claimSeconds)
        {
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("winner", winner),
                new KeyValuePair<string, string>("game", raffleName),
                new KeyValuePair<string, string>("donator", donator),
                new KeyValuePair<string, string>("linker", linker),
                new KeyValuePair<string, string>("claimtime",claimSeconds.ToString()),
                new KeyValuePair<string, string>("entries", entries)
            };
            JToken raffleServerDetails = JObject.Parse(await request(requestType.GET, Properties.Settings.Default.webAddRaffleWinner, args)).SelectToken("raffle_win");
            Objects.RaffleWin raffleWin = new Objects.RaffleWin(raffleServerDetails);
            return raffleWin;
        }

        // Downloads all emotes (subscriber, global, BTTV) and loads them into a list; returns list
        public async static Task<List<Objects.Emote>> downloadGlobalEmotes()
        {
            if(!File.Exists("subscriber_emotes.json") || DateTime.UtcNow - File.GetLastWriteTimeUtc("subscriber_emotes.json") > TimeSpan.FromDays(3))
                File.WriteAllText("subscriber_emotes.json", await request(requestType.GET, "https://twitchemotes.com/api_cache/v2/subscriber.json"));
            if (!File.Exists("global_emotes.json") || DateTime.UtcNow - File.GetLastWriteTimeUtc("global_emotes.json") > TimeSpan.FromDays(3))
                File.WriteAllText("global_emotes.json", await request(requestType.GET, "https://twitchemotes.com/api_cache/v2/global.json"));
            if (!File.Exists("bttv_emotes.json") || DateTime.UtcNow - File.GetLastWriteTimeUtc("bttv_emotes.json") > TimeSpan.FromDays(3))
                File.WriteAllText("bttv_emotes.json", await request(requestType.GET, "https://api.betterttv.net/2/emotes"));
            string sub_emotes = File.ReadAllText("subscriber_emotes.json");
            string global_emotes = File.ReadAllText("global_emotes.json");
            string bttv_emotes = File.ReadAllText("bttv_emotes.json");
            List<Objects.Emote> emotes = new List<Objects.Emote>();
            //Parse sub emotes
            foreach(JToken sub in JObject.Parse(sub_emotes).SelectToken("channels"))
                foreach(JToken details in sub)
                    foreach(JToken emote in details.SelectToken("emotes"))
                        emotes.Add(new Objects.Emote(emote.SelectToken("code").ToString(), details.SelectToken("id").ToString(), true));
            //Parse global emotes
            JToken gEmotes = JObject.Parse(global_emotes).SelectToken("emotes");
            foreach (JToken emote in gEmotes)
                foreach(JToken part in emote)
                    emotes.Add(new Objects.Emote(((JProperty)part.Parent).Name, "global_emote", false));
            //Parse bettertwitchtv emotes
            JToken bttvEmotes = JObject.Parse(bttv_emotes).SelectToken("emotes");
            foreach(JToken emote in bttvEmotes)
                emotes.Add(new Objects.Emote(emote.SelectToken("code").ToString(), "bttv_emote", false));
            Common.initialize("Emotes loaded into memory: " + emotes.Count);
            return emotes;
        }

        // Downloads a streamer's most recent game asynchonously from TwitchLib API and returns it
        public async static Task<string> getStreamersMostRecentGame(string streamer)
        {
            TwitchLib.TwitchChannel channelData = await TwitchLib.TwitchAPI.getTwitchChannel(streamer);
            return channelData.Game;
        }

        // Performs GET request to BurkeBlack.TV detailing a new highlight to be mad
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
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("name", username),
                    new KeyValuePair<string, string>("title", title),
                    new KeyValuePair<string, string>("stream_created_at", Common.StreamRefresher.Stream.CreatedAt),
                    new KeyValuePair<string, string>("time_in", timeIn)
                };
                if (!Common.DryRun)
                    await request(requestType.GET, Properties.Settings.Default.webCreateHighlight, args);
                return true;
            }
            return false;
        }

        // Downloads a sub's personal command and returns the string value
        public async static Task<string> getSetPersonalCommand(string username, string newData)
        {
            if(newData == null)
            {
                //Fetch current data
                foreach(Objects.PersonalCommand command in Common.PersonalCommands)
                    if (command.Username.ToLower() == username.ToLower())
                        return command.Data;
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("name", username),
                    new KeyValuePair<string, string>("type", "fetch")
                };
                string personalCommandData = await request(requestType.GET, Properties.Settings.Default.webPersonalCommand, args);
                Common.PersonalCommands.Add(new Objects.PersonalCommand(username, personalCommandData));
                return personalCommandData;
            } else
            {
                //Set new data
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("name", username),
                    new KeyValuePair<string, string>("type", "update"),
                    new KeyValuePair<string, string>("newData", newData)
                };
                if (!Common.DryRun)
                    await request(requestType.GET, Properties.Settings.Default.webPersonalCommand, args);
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
                    Common.PersonalCommands.Add(new Objects.PersonalCommand(username, newData));
                return string.Format("[{0}] Your personal command has been updated!", username);
            }
        }

        // Queries talk server with contents of twitch message, returns string value
        public async static Task<string> talk(string username, string message)
        {
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("message", message)
            };
            string resp = await request(requestType.GET, Properties.Settings.Default.webTalk, args);
            return string.Format("[{0}] {1}", username, resp);
        }

        // Downloads and loads multihost streamers into list; returns list
        public async static Task<List<Host>> downloadMultihostStreamers()
        {
            List<Host> streamers = new List<Host>();
            string jsonStr = await request(requestType.GET, Properties.Settings.Default.webMultihostStreamers);
            foreach (JToken streamer in JObject.Parse(jsonStr).SelectToken("streamers"))
                streamers.Add(new Host(streamer.SelectToken("streamer").ToString(), streamer.SelectToken("information").ToString()));
            return streamers;
        }

        // Queries Twitch API for each streamer in the list to get a list of online streamers; returns list
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

        // Downloads and loads users to notify; returns list
        public async static Task<List<string>> downloadUsersToNotify()
        {
            List<string> usersToNotify = new List<string>();
            string jsonStr = await request(requestType.GET, Properties.Settings.Default.webNotifyMeUsers);
            foreach(JToken user in JObject.Parse(jsonStr).SelectToken("users"))
                usersToNotify.Add(user.SelectToken("username").ToString());
            return usersToNotify;
        }

        // Queries BurkeBlack.TV to add user to notify list
        public async static void addNotifyUser(string username)
        {
            if (!Common.DryRun)
            {
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("name", username)
                };
                await request(requestType.GET, Properties.Settings.Default.webAddNotifyMeUser, args);
            }
        }

        // Queries BurkeBlack.TV to disable notifications for user
        public async static void removeNotifyUser(string username)
        {
            if (!Common.DryRun)
            {
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("name", username)
                };
                await request(requestType.GET, Properties.Settings.Default.webRemoveNotifyMeUser, args);
            }
        }

        // Performs query to add doubloons to a user
        public async static void addDoubloons(string username, int amount)
        {
            if (!Common.DryRun)
            {
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("name", username),
                    new KeyValuePair<string, string>("amount", amount.ToString())
                };
                await request(requestType.GET, Properties.Settings.Default.webAddDoubloons, args);
            }
        }

        // Performs query to add soundbyte credits to a user
        public async static void addSoundbyteCredits(string username, int amount)
        {
            if (!Common.DryRun)
            {
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("name", username),
                    new KeyValuePair<string, string>("amount", amount.ToString())
                };
                await request(requestType.GET, Properties.Settings.Default.webAddSoundbyteCredits, args);
            }
        }

        // Performs query to distribute doubloons to all viewers
        public async static Task<bool> distibuteDoubloons(int amount)
        {
            if (Common.DryRun)
                return true;
            try
            {
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("amount", amount.ToString())
                };
                await request(requestType.GET, Properties.Settings.Default.webDistributeDoubloons, args);
                return true;
            }catch (Exception)
            {
                return false;
            }
        }

        // Performs GET query to upload message counts
        public async static void uploadChatMessageCounts(string userMessageCounts)
        {
            if (!Common.DryRun)
            {
                List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("data", userMessageCounts)
                };
                await request(requestType.GET, Properties.Settings.Default.webAddChatMessages, args);
            }
        }

        // Downloads all TLDS and loads them into list; returns list
        public async static Task<List<string>> downloadTopLevelDomains()
        {
            List<string> tlds = new List<string>();
            string cnts = await request(requestType.GET, "https://data.iana.org/TLD/tlds-alpha-by-domain.txt");
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

        // Performs query to get a specific user's doubloon count; returns string value
        public async static Task<string> getUserDoubloons(string username)
        {
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("user", username)
            };
            return await request(requestType.GET, Properties.Settings.Default.webDoubloonCount, args);
        }

        // Downloads bot update details; returns UpdateDetails object
        public async static Task<UpdateDetails> downloadUpdateDetails()
        {
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("current_version", Assembly.GetExecutingAssembly().GetName().Version.ToString())
            };
            string cnts =  await request(requestType.GET, Properties.Settings.Default.webUpdateRequest, args);
            return new UpdateDetails(JObject.Parse(cnts).SelectToken("update_data"));
        }

        // Query to download a file (used for updates)
        public static void downloadFile(string onlineLocation, string fileName)
        {
            new System.Net.WebClient().DownloadFile(onlineLocation, fileName);
        }

        // Downloads and loads the last 10 donations into a list; returns list
        public async static Task<List<Objects.RecentDonation>> downloadRecentDonations()
        {
            List<Objects.RecentDonation> recentDonations = new List<Objects.RecentDonation>();
            string jsonStr = "";
            try
            {
                jsonStr = await request(requestType.GET, Properties.Settings.Default.webUpdateDonations);
            }
            catch (Exception) { return null; }
            foreach(JToken donation in JObject.Parse(jsonStr).SelectToken("donations"))
                recentDonations.Add(new Objects.RecentDonation(donation));
            return recentDonations;
        }

        // Downloads Burke's latest youtube video from YT API; returns YoutubeVideo object
        public async static Task<Objects.YoutubeVideo> getBurkesLatestYTVideo()
        {
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("key", Properties.Settings.Default.YoutubeAPIKey),
                new KeyValuePair<string, string>("channelId", "UCmW5CTVpCWDM55hZwxtpjyw"),
                new KeyValuePair<string, string>("part", "snippet,id"),
                new KeyValuePair<string, string>("order", "date"),
                new KeyValuePair<string, string>("maxResults", "1")
            };
            string jsonStr = await request(requestType.GET, "https://www.googleapis.com/youtube/v3/search", args);
            string videoID = JObject.Parse(jsonStr).SelectToken("items")[0].SelectToken("id").SelectToken("videoId").ToString();
            string videoDescription = JObject.Parse(jsonStr).SelectToken("items")[0].SelectToken("snippet").SelectToken("description").ToString();
            string videoTitle = JObject.Parse(jsonStr).SelectToken("items")[0].SelectToken("snippet").SelectToken("title").ToString();
            args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("key", Properties.Settings.Default.YoutubeAPIKey),
                new KeyValuePair<string, string>("id", videoID),
                new KeyValuePair<string, string>("part", "statistics")
            };
            jsonStr = await request(requestType.GET, "https://www.googleapis.com/youtube/v3/videos", args);
            JToken stats = JObject.Parse(jsonStr).SelectToken("items")[0].SelectToken("statistics");
            return new Objects.YoutubeVideo(videoTitle, videoDescription, videoID, int.Parse(stats.SelectToken("viewCount").ToString()), int.Parse(stats.SelectToken("likeCount").ToString()), int.Parse(stats.SelectToken("dislikeCount").ToString()),
                int.Parse(stats.SelectToken("favoriteCount").ToString()), int.Parse(stats.SelectToken("commentCount").ToString()));
        }

        // Downloads Burke's YouTube channel stats, returns YoutubeStats object
        public async static Task<Objects.YoutubeStats> getYoutubeChannelStats(string channel)
        {
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("key", Properties.Settings.Default.YoutubeAPIKey),
                new KeyValuePair<string, string>("id", "UCmW5CTVpCWDM55hZwxtpjyw"),
                new KeyValuePair<string, string>("part", "statistics")
            };
            string jsonStr = await request(requestType.GET, "https://www.googleapis.com/youtube/v3/channels", args);
            return new Objects.YoutubeStats(JObject.Parse(jsonStr).SelectToken("items")[0].SelectToken("statistics"));
        }

        // Performs query to send push notification to Swifty's phone
        public async static void notifySwifty(string title, string descriptor)
        {
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("title", title),
                new KeyValuePair<string, string>("descriptor", descriptor)
            };
            await request(requestType.GET, Properties.Settings.Default.webNotify, args);
        }

        // Downloads follow data for a specific channel; returns FollowData object
        public async static Task<Objects.FollowData> getFollowData(string channel)
        {
            TwitchLib.TwitchChannel twitchChannel;
            try
            {
                twitchChannel = await TwitchLib.TwitchAPI.getTwitchChannel(channel);
            } catch(Exception)
            {
                return null;
            }
            return new Objects.FollowData(twitchChannel);
        }

        // Queries Discord API to generate 1 use, 2 minute Discord invite code; returns discord invite string value
        public static string createInviteCode()
        {
            string url = Endpoints.BaseAPI + Endpoints.Channels + $"/{Common.DiscordClient.GetChannelByName("general").ID}" + Endpoints.Invites;
            try
            {
                var response = discordPost(url, DiscordClient.token, "{\"max_age\":120,\"max_uses\":1,\"temporary\":false,\"xkcdpass\":false}");
                return JObject.Parse(response).SelectToken("code").ToString();
            }
            catch (Exception)
            {
                return "Failed!";
            }
        }

        // Ripped from DiscordSharp's WebWrapper.Post internal function.  Needed for creating custom invite link
        private static string discordPost(string url, string token, string message, bool acceptInviteWorkaround = false)
        {
            string UserAgentString = $"DiscordBot (http://github.com/Luigifan/DiscordSharp, {typeof(DiscordClient).Assembly.GetName().Version.ToString()})";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Headers["authorization"] = token;
            if (acceptInviteWorkaround)
                httpRequest.ContentLength = message.Length;
            httpRequest.ContentType = "application/json";
            httpRequest.Method = "POST";
            httpRequest.UserAgent += $" {UserAgentString}";
            if (!string.IsNullOrEmpty(message))
            {
                using (var sw = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    sw.Write(message);
                    sw.Flush();
                    sw.Close();
                }
            }
            try
            {
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var sr = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = sr.ReadToEnd();
                    if (!string.IsNullOrEmpty(result))
                    {
                        JObject jsonTest = JObject.Parse(result);
                        if (jsonTest != null)
                        {
                            if (!jsonTest["bucket"].IsNullOrEmpty()) //you got rate limited punk
                            {
                                Task.Delay(jsonTest["retry_after"].ToObject<int>()).Wait(); //wait
                                discordPost(url, token, message, acceptInviteWorkaround); //try again
                            }
                        }
                    }
                    if (result != "")
                        return result;
                }
            }
            catch (WebException e)
            {
                throw e;
            }
            return "";
        }

        // Performs query to add invite code to listing in database so users cant generate more than one
        public async static void addInviteCode(string username, string code)
        {
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("user", username),
                new KeyValuePair<string, string>("code", code)
            };
            await request(requestType.GET, Properties.Settings.Default.webAddInvite, args);
        }

        // Performs query to pull existing discord invite from database; returns string value
        public async static Task<string> getExistingDiscordInvite(string username)
        {
            List<KeyValuePair<string, string>> args = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("user", username)
            };
            string jsonStr = await request(requestType.GET, Properties.Settings.Default.webDiscordOldInvite, args);
            JToken json = JObject.Parse(jsonStr).SelectToken("discord_invite");
            if (json.SelectToken("found").ToString() == "True")
                return json.SelectToken("invite").ToString();
            else
                return null;
        }

        // Private web request function, performs GET or POST calls, returns downloaded string value
        private async static Task<string> request(requestType type, string host, List<KeyValuePair<string, string>> args = null, bool utf8 = true)
        {
            switch(type)
            {
                case requestType.GET:
                    string url = "";
                    if (args != null)
                        foreach (KeyValuePair<string, string> arg in args)
                            if (url == "")
                                url = string.Format("?{0}={1}", arg.Key, arg.Value);
                            else
                                url = string.Format("{0}&{1}={2}", url, arg.Key, arg.Value);
                    url = host + url;
                    return await get(url, utf8);
                case requestType.POST:
                    return await post(host, args, utf8);
                default:
                    return null;
            }
        }

        // Private web GET request; returns downloaded string value
        private async static Task<string> get(string url, bool utf8 = false)
        {
            Common.other("QUERY: " + url);
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                if (utf8)
                    wc.Encoding = Encoding.UTF8;
                return await wc.DownloadStringTaskAsync(url);
            } catch (Exception)
            {
                return "";
            }
        }

        // Private web POST request; returns download string value
        private async static Task<string> post(string host, List<KeyValuePair<string, string>> args, bool utf8)
        {
            Common.other("QUERY (POST): " + host);
            if(args == null) { args = new List<KeyValuePair<string, string>>(); }
            HttpClient client = new HttpClient();
            FormUrlEncodedContent body = new FormUrlEncodedContent(args);
            HttpResponseMessage response = await client.PostAsync(host, body);
            if (response.IsSuccessStatusCode)
                return await new StreamReader(await response.Content.ReadAsStreamAsync()).ReadToEndAsync();
            return null;
        }

        // Web request type distinguisher
        private enum requestType
        {
            GET,
            POST
        }
    }
}
