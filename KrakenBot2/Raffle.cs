using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Timers;

namespace KrakenBot2
{
    public class Raffle
    {
        private RaffleProperties raffleProperties;
        private List<string> enteredViewers = new List<string>();

        //Minute interval timers
        private Timer raffleTimer = new Timer(60000);
        private Timer claimTimer = new Timer(1000);

        //Raffle instance exclusive variables
        private string activeWinner;

        public string RaffleName { get { return raffleProperties.Raffle_Name; } }
        public string RaffleDonator { get { return raffleProperties.Raffle_Donator; } }
        public string RaffleAuthor { get { return raffleProperties.Raffle_Author; } }

        public Raffle(JToken giveawayProperties)
        {
            raffleProperties = new RaffleProperties(giveawayProperties);
            raffleTimer.Elapsed += raffleTimerTick;
            claimTimer.Elapsed += claimTimerTick;
        }

        //Starts the raffle
        public void startRaffle()
        {
            //Ensures this instance of 'Raffle' is not active
            if(!raffleIsActive())
            {
                //Sends disclaimer bullshit to channel chat
                Common.ChatClient.sendMessage("DISCLAIMER: This is a promotion from BurkeBlack. Twitch does not sponsor or endorse broadcaster promotions and is not responsible for them.", Common.DryRun);
                //Sends name of giveaway, who donated it, and which mod that sent it
                Common.ChatClient.sendMessage(string.Format("/me Giveaway for: {0}; Donated by: {1}; Submitted by: {2}", raffleProperties.Raffle_Name, raffleProperties.Raffle_Donator, raffleProperties.Raffle_Author), Common.DryRun);
                //Now we need to send a different message based on the type of giveaway it is and include giveaway length
                switch(raffleProperties.Raffle_Type)
                {
                    case Common.GiveawayTypes.EXGAMES:
                        Common.ChatClient.sendMessage(string.Format("/me Giveaway Type: !GAMES; Giveaway Length: {0} minutes. There are currently {1} !games available on BurkeBlack.TV!", raffleProperties.Raffle_Length, raffleProperties.ExGamesCount), Common.DryRun);
                        break;
                    case Common.GiveawayTypes.HUMBLEBUNDLE:
                        Common.ChatClient.sendMessage(string.Format("/me Giveaway Type: HUMBLEBUNDLE; Giveaway Length: {0} minutes", raffleProperties.Raffle_Length), Common.DryRun);
                        break;
                    case Common.GiveawayTypes.LOGITECH:
                        Common.ChatClient.sendMessage(string.Format("/me Giveaway Type: LOGITECH; Giveaway Length: {0} minutes", raffleProperties.Raffle_Length), Common.DryRun);
                        break;
                    case Common.GiveawayTypes.ORIGINCODE:
                        Common.ChatClient.sendMessage(string.Format("/me Giveaway Type: ORIGIN CODE; Giveaway Length: {0} minutes", raffleProperties.Raffle_Length), Common.DryRun);
                        break;
                    case Common.GiveawayTypes.OTHER:
                        //If the giveaway is 'other', we should have a specifier sent from the panel, we will send that to chat
                        Common.ChatClient.sendMessage(string.Format("/me Giveaway Type: OTHER [{0}]; Giveaway Length: {1} minutes", raffleProperties.Raffle_Type_Other, raffleProperties.Raffle_Length), Common.DryRun);
                        break;
                    case Common.GiveawayTypes.SERIALCODE:
                        Common.ChatClient.sendMessage(string.Format("/me Giveaway Type: SERIAL/CODE; Giveaway Length: {0} minutes", raffleProperties.Raffle_Length), Common.DryRun);
                        break;
                    case Common.GiveawayTypes.SOUND_BYTES:
                        //If the giveaway is 'sound_bytes', we should let viewers know how many
                        Common.ChatClient.sendMessage(string.Format("/me Giveaway Type: SOUND BYTES [{0}]; Giveaway Length: {1} minutes", raffleProperties.Soundbyte_Count, raffleProperties.Raffle_Length), Common.DryRun);
                        break;
                    case Common.GiveawayTypes.STEAMCODE:
                        Common.ChatClient.sendMessage(string.Format("/me Giveaway Type: STEAM CODE; Giveaway Length: {0} minutes", raffleProperties.Raffle_Length), Common.DryRun);
                        break;
                    case Common.GiveawayTypes.STEAMGIFT:
                        Common.ChatClient.sendMessage(string.Format("/me Giveaway Type: STEAM GIFT; Giveaway Length: {0} minutes", raffleProperties.Raffle_Length), Common.DryRun);
                        break;
                    case Common.GiveawayTypes.STEAMTRADE:
                        Common.ChatClient.sendMessage(string.Format("/me Giveaway Type: STEAM TRADE; Giveaway Length: {0} minutes", raffleProperties.Raffle_Length), Common.DryRun);
                        break;
                    default:
                        //We also want to throw a message if the giveaway is not identified (perhaps something a bit more mature)
                        Common.ChatClient.sendMessage(string.Format("/me Giveaway Type: NOT SET; Giveaway Length: {0} minutes", raffleProperties.Raffle_Length), Common.DryRun);
                        break;
                }
                //Next step is to check for absolute filters
                string requiredFilter = "NONE";
                if (raffleProperties.Sub_Only)
                    requiredFilter = "SUBSCRIBER";
                else if (raffleProperties.Follower_Only)
                    requiredFilter = "FOLLOWER";
                //Next step is to check for doubloon/minute requirements;
                switch(raffleProperties.Raffle_Filter)
                {
                    case RaffleProperties.Filters.DOUBLOONS:
                        Common.ChatClient.sendMessage(string.Format("/me Applied Filter: DOUBLOONS [{0}]; Required Status: {1}", raffleProperties.Raffle_Filter_Amount, requiredFilter), Common.DryRun);
                        break;
                    case RaffleProperties.Filters.MINUTES:
                        Common.ChatClient.sendMessage(string.Format("/me Applied Filter: MINUTES [{0}]", raffleProperties.Raffle_Filter_Amount), Common.DryRun);
                        break;
                    case RaffleProperties.Filters.NONE:
                        if (requiredFilter != "NONE")
                            Common.ChatClient.sendMessage(string.Format("Required Status: {0}", requiredFilter), Common.DryRun);
                        break;
                }
                //Finally we want to say how to enter, start the raffle, and log to debug console that a raffle was started
                Common.ChatClient.sendMessage("To enter giveaway, type: !enter", Common.DryRun);
                Common.notify("New GIVEAWAY; from: " + raffleProperties.Raffle_Author, "Game: " + raffleProperties.Raffle_Name + ", from: " + raffleProperties.Raffle_Donator);
                Common.DiscordClient.SendMessageToChannel(string.Format("NEW GIVEAWAY: Name: {0}, Donator: {1}, Author: {2}", raffleProperties.Raffle_Name, raffleProperties.Raffle_Donator, raffleProperties.Raffle_Author), Common.DiscordClient.GetChannelByName("kraken-relay"));
                raffleTimer.Start();
            }

        }

        //Perform sanity checks and adds viewer to active raffle
        public bool addEntry(string newViewer)
        {
            if(!raffleIsActive())
            {
                System.Threading.Thread.Sleep(500);
                Common.ChatClient.sendMessage(string.Format(".timeout {0} 1", newViewer));
                Common.ChatClient.sendMessage(string.Format("Please do not enter when a raffle is not active, {0}", newViewer));
                return false;
            }
            newViewer = newViewer.ToLower();
            //Check to see if viewer has already entered
            foreach(string viewer in enteredViewers)
            {
                if (viewer == newViewer)
                    return false;
            }
            //Check to see if viewer has already hit max wins for giveaway type
            foreach(Objects.PreviousRaffleWinner viewer in raffleProperties.Previous_Winners)
            {
                if(viewer.Username.ToLower() == newViewer && viewer.AffectWinLimit)
                {
                    if (raffleProperties.Raffle_Type == Common.GiveawayTypes.EXGAMES && viewer.GiveawayType == Common.GiveawayTypes.EXGAMES)
                        return false;
                    else if (raffleProperties.Raffle_Type != Common.GiveawayTypes.EXGAMES && viewer.GiveawayType != Common.GiveawayTypes.EXGAMES)
                        return false;
                }
                if (viewer.Username.ToLower() == newViewer)
                    return false;
            }
            Common.initialize("New entry: " + newViewer);
            enteredViewers.Add(newViewer);
            Common.WhisperClient.sendWhisper(newViewer, "Your entry has been confirmed! Good luck!", Common.DryRun);
            return true;
        }

        //Performs attempt to claim
        private int invalidClaimTimeoutDuration = TimeoutConverter.Hours(12);
        public bool tryClaim(string username)
        {
            if(raffleIsActive())
            {
                if(activeWinner.ToLower() == username.ToLower())
                {
                    claimTimer.Stop();
                    string entries = "";
                    foreach(string entry in enteredViewers)
                    {
                        if (entries == "")
                            entries = entry;
                        else
                            entries = entries + "|" + entry;
                    }
                    Objects.RaffleWin raffleWin = WebCalls.addRaffleWinner(activeWinner, raffleProperties.Raffle_Name, raffleProperties.Raffle_Donator,
                        entries, raffleProperties.Raffle_Linker, claimCurrentSecond).Result;
                    Common.ChatClient.sendMessage(string.Format("GIVEAWAY CLAIMED BY: {0}! You've won {1} raffles, and entered {2} (that's a win percentage of {3}%). Your claim time was {4} seconds," +
                        " however your average claim time is {5} seconds.", raffleWin.Winner, raffleWin.WinCount, raffleWin.EnterCount, raffleWin.WinPercentage, raffleWin.ClaimTime, raffleWin.ClaimTimeAvg), Common.DryRun);
                    sendWinnerWhisper();
                    if (raffleProperties.Raffle_Type == Common.GiveawayTypes.SOUND_BYTES)
                    {
                        WebCalls.addSoundbyteCredits(raffleWin.Winner, raffleProperties.Soundbyte_Count);
                        Common.ChatClient.sendMessage(string.Format("[Auto] Added {0} soundbyte credits to {1}'s total.", raffleProperties.Soundbyte_Count, raffleWin.Winner), Common.DryRun);
                    }
                    int latestID = WebCalls.downloadRaffleID().Result;
                    if (raffleProperties.Raffle_Type == Common.GiveawayTypes.EXGAMES)
                        Common.ChatClient.sendMessage(string.Format("Giveaway details can be viewed here: http://burkeblack.tv/giveaways/listing.php?gid={0} .  Video on how automatic !games giveaways work: http://www.twitch.tv/burkeblack/c/5663793", latestID.ToString()));
                    else
                        Common.ChatClient.sendMessage(string.Format("Giveaway details can be viewed here: http://burkeblack.tv/giveaways/listing.php?gid={0}", latestID.ToString()), Common.DryRun);
                    Common.notify("NEW GIVEAWAY CLAIM", raffleWin.Winner + " claimed " + raffleProperties.Raffle_Name);
                    Common.DiscordClient.SendMessageToChannel(string.Format("NEW CLAIM: Winner: {0}, Name: {1}, Donator: {2}, Author: {3}", raffleWin.Winner, raffleProperties.Raffle_Name, raffleProperties.Raffle_Donator, raffleProperties.Raffle_Author), Common.DiscordClient.GetChannelByName("kraken-relay"));
                    return true;
                } else
                {
                    System.Threading.Thread.Sleep(400);
                    Common.ChatClient.sendMessage(string.Format("*WARNING* [{0}] Timed out for 12 hours. [Invalid Claim])", username), Common.DryRun);
                    Common.WhisperClient.sendWhisper(username, "You have been timedout for 12 hours.  Do NOT try to claim a giveaway that you did not win.", Common.DryRun);
                    Common.ChatClient.sendMessage(string.Format(".timeout {0} {1}", username, invalidClaimTimeoutDuration), Common.DryRun);
                }
            }
            return false;
        }

        private int invalidPassTimeoutHours = 1;
        public bool tryPass(string username)
        {
            int invalidPassTimeoutDuration = TimeoutConverter.Hours(invalidPassTimeoutHours);
            if (username.ToLower() == activeWinner.ToLower() && raffleIsActive())
            {
                Common.ChatClient.sendMessage(string.Format("Winner ({0}) has passed on their win, redrawing...", activeWinner), Common.DryRun);
                Common.WhisperClient.sendWhisper(activeWinner, "Thank you for passing on your giveaway.  burkeEpic", Common.DryRun);
                processDraw(true);
                return true;
            } else
            {
                System.Threading.Thread.Sleep(400);
                Common.ChatClient.sendMessage(string.Format("*WARNING* [{0}] Timed out for 1 hour. [Invalid Pass])", username), Common.DryRun);
                Common.WhisperClient.sendWhisper(username, "You have been timedout for 1 hour.  Do NOT try to pass a giveaway that you did not win.", Common.DryRun);
                Common.ChatClient.sendMessage(string.Format(".timeout {0} {1}", username, invalidPassTimeoutDuration), Common.DryRun);
                return false;
            }
        }

        private void sendWinnerWhisper()
        {
            if(raffleProperties.Raffle_Linker != "")
            {
                Common.WhisperClient.sendWhisper(activeWinner, "Congrats on your win! Visit http://burkeblack.tv , login with your Twitch account and scroll to Prizes/Rewards.  Click on the Raffle Prizes tab to view your raffle prize!", Common.DryRun);
            } else
            {
                switch(raffleProperties.Raffle_Type)
                {
                    case Common.GiveawayTypes.EXGAMES:
                        Common.WhisperClient.sendWhisper(activeWinner, "Congrats on your win! Visit http://burkeblack.tv and login with your Twitch account.  A message box will detail instructions on how to claim your !games game!", Common.DryRun);
                        break;
                    default:
                        Common.WhisperClient.sendWhisper(activeWinner, "Congrats on your win! A mod will get in contact with you shortly!", Common.DryRun);
                        break;
                }
            }
        }

        //Returns whether or not raffle is active
        public bool raffleIsActive()
        {
            return raffleTimer.Enabled || claimTimer.Enabled;
        }

        private bool processDraw(bool redraw = false)
        {
            return processDrawAsync(redraw).Result;
        }

        private async Task<bool> processDrawAsync(bool redraw = false)
        {
            //Validate drawing process
            if (redraw == true)
                enteredViewers.Remove(activeWinner);
            if (enteredViewers.Count < raffleProperties.Raffle_Minimum_Entries)
            {
                Common.ChatClient.sendMessage(string.Format("/me GIVEAWAY ERROR: Entry count below minimum ({0}) required for giveaway to proceed. [FATAL]", raffleProperties.Raffle_Minimum_Entries), Common.DryRun);
                return false;
            }
            //Check filters
            List<string> filteredEntries = null;
            List<string> masterList = enteredViewers;
            switch (raffleProperties.Raffle_Filter)
            {
                case RaffleProperties.Filters.DOUBLOONS:
                    filteredEntries = await WebCalls.downloadDoubloonEntries(raffleProperties.Raffle_Filter_Amount, enteredViewers);
                    masterList = filteredEntries;
                    break;
                case RaffleProperties.Filters.MINUTES:
                    filteredEntries = await WebCalls.downloadMinuteEntries(raffleProperties.Raffle_Filter_Amount, enteredViewers);
                    masterList = filteredEntries;
                    break;
                case RaffleProperties.Filters.NONE:
                    break;
            }
            //Validate filters
            if(raffleProperties.Raffle_Filter != RaffleProperties.Filters.NONE && (filteredEntries == null || filteredEntries.Count < raffleProperties.Raffle_Minimum_Entries))
            {
                Common.ChatClient.sendMessage("/me GIVEAWAY ERROR: Filtered raffle entry results failed or falls below minimum required entries.  Resorting to original entry list [NON-FATAL]", Common.DryRun);
                masterList = enteredViewers;
            }
            //Draw winner and perform follower/sub only checks
            bool resultFound = false;
            string winner = "";
            //Create local list of non-blocked users
            List<string> drawFromList = new List<string>();
            foreach(string entry in masterList)
            {
                if (!raffleProperties.Blocked_Viewers.Contains(entry))
                    drawFromList.Add(entry);
            }
            while (resultFound == false)
            {
                if(drawFromList.Count < raffleProperties.Raffle_Minimum_Entries)
                {
                    resultFound = true;
                    Common.ChatClient.sendMessage("/me GIVEAWAY ERROR: Entry count has fallen below required minimum entry count. [FATAL]", Common.DryRun);
                    return false;
                }

                winner = drawFromList[new Random().Next(0, drawFromList.Count)];
                if (raffleProperties.Follower_Only)
                    if (!await TwitchLib.TwitchAPI.userFollowsChannel(winner, "burkeblack"))
                    {
                        drawFromList.Remove(winner);
                        Common.ChatClient.sendMessage(string.Format("Winner ({0}) does not follow BurkeBlack! Redrawing..."), Common.DryRun);
                        continue;
                    }
                if (raffleProperties.Sub_Only)
                    if (!TwitchLib.TwitchAPI.channelHasUserSubscribed(winner, "burkeblack", Properties.Settings.Default.BurkeOAuth))
                    {
                        drawFromList.Remove(winner);
                        Common.ChatClient.sendMessage(string.Format("Winner ({0}) is not subscribed BurkeBlack! Redrawing..."), Common.DryRun);
                        continue;
                    }
                resultFound = true;
            }
            if(!redraw)
                Common.ChatClient.sendMessage(string.Format("/me GIVEAWAY WINNER: {0} (out of {1} total entries, draw percentage: {2}%)", winner, masterList.Count, (Math.Round(((double)1 / masterList.Count), 2) * 100)), Common.DryRun);
            else
                Common.ChatClient.sendMessage(string.Format("/me GIVEAWAY REDRAW WINNER: {0} (out of {1} total entries, draw percentage: {2}%)", winner, masterList.Count, (Math.Round(((double)1 / masterList.Count), 2) * 100)), Common.DryRun);
            Common.ChatClient.sendMessage(string.Format("/me You have {0} minutes to claim your giveaway, {1}. Use !claim to claim. Use !pass to pass on the giveaway and have the bot draw a new winner.", raffleProperties.Raffle_Claim_Length, winner), Common.DryRun);
            claimCurrentSecond = 0;
            activeWinner = winner;
            claimTimer.Start();
            Common.initialize("Claim timer started.");
            return true;
        }

        //Processes an event where the user fails to claim (
        private void processNoClaim()
        {
            claimCurrentSecond = 0;
            if(processDraw(true))
                claimTimer.Start();
        }

        #region TIMERS
        int raffleCurrentMinute = 0;
        //Ticks every minute, will tick max 5 times (dynamic)
        private void raffleTimerTick(object sender, ElapsedEventArgs e)
        {
            if (raffleCurrentMinute == (raffleProperties.Raffle_Length - 1))
            {
                raffleTimer.Stop();
                claimTimer.Start();
                Common.ChatClient.sendMessage("GIVEAWAY TIMEUP", Common.DryRun);
                processDraw();
            } else if(raffleCurrentMinute == (raffleProperties.Raffle_Length - 2))
            {
                Common.ChatClient.sendMessage(string.Format("/me Giveaway is for: {0}, donated by: {1}", raffleProperties.Raffle_Name, raffleProperties.Raffle_Donator), Common.DryRun);
                Common.ChatClient.sendMessage(string.Format("Entries so far: {0}; Time Remaining: {1} minute", enteredViewers.Count, 1), Common.DryRun);
                if (raffleProperties.Raffle_Type == Common.GiveawayTypes.EXGAMES)
                    Common.ChatClient.sendMessage(string.Format("There are currently {0} !games available on BurkeBlack.TV!", raffleProperties.ExGamesCount), Common.DryRun);
            } else if(raffleCurrentMinute == (raffleProperties.Raffle_Length - 4))
            {
                Common.ChatClient.sendMessage(string.Format("/me Giveaway is for: {0}, donated by: {1}", raffleProperties.Raffle_Name, raffleProperties.Raffle_Donator), Common.DryRun);
                Common.ChatClient.sendMessage(string.Format("Entries so far: {0}; Time Remaining: {1} minutes", enteredViewers.Count, 3), Common.DryRun);
                //if (raffleProperties.Raffle_Type == Common.GiveawayTypes.EXGAMES)
                    //Common.ChatClient.sendMessage(string.Format("There are currently {0} !games available on BurkeBlack.TV!", raffleProperties.ExGamesCount), Common.DryRun);
            }
            raffleCurrentMinute++;
            Common.other("Minutes passed: " + raffleCurrentMinute);
        }
        //Ticks every minute, will tick max 120 times
        int claimCurrentSecond = 0;
        private void claimTimerTick(object sender, ElapsedEventArgs e)
        {
            if(claimCurrentSecond == ((raffleProperties.Raffle_Claim_Length - 1) * 60))
            {
                Common.ChatClient.sendMessage(string.Format("/me You have one minute to claim your giveaway, {0}. Use !claim to claim the giveaway.", activeWinner), Common.DryRun);
            } else if(claimCurrentSecond == (raffleProperties.Raffle_Claim_Length * 60))
            {
                claimTimer.Stop();
                Common.ChatClient.sendMessage(string.Format("Winner ({0}) has failed to claim their giveaway. Redrawing...", activeWinner), Common.DryRun);
                processNoClaim();
            }
            claimCurrentSecond++;
        }
        #endregion

        #region DOWNLOAD FUNCTIONS
        //Downloads number of !games available
        private async Task<int> downloadExGamesCount()
        {
            return await WebCalls.downloadExGamesCount();
        }

        //Downloads users that meet minutes watched criteria
        private async Task<List<string>> downloadMinuteEntries(int minutes, List<string> entries)
        {
            return await WebCalls.downloadMinuteEntries(minutes, entries);
        }

        //Downloads users that meet doubloons criteria
        private async Task<List<string>> downloadDoubloonEntries(int doubloons, List<string> entries)
        {
            return await WebCalls.downloadDoubloonEntries(doubloons, entries);
        }

        //Downloads the ID of a given raffle so as to return a URL with the giveaway results
        private async Task<int> downloadRaffleID()
        {
            return await WebCalls.downloadRaffleID();
        }
        #endregion


        //Contains all raffle properties downloaded from transition bot server
        private class RaffleProperties
        {
            

            public enum Filters
            {
                NONE,
                DOUBLOONS,
                MINUTES
            }

            private int raffleLength, raffleClaimLength, raffleMinimumEntries, raffleFilterAmount, raffleSteamID, raffleSoundbyteCount;
            private string raffleDonator, raffleName, raffleAuthor, raffleLinker, raffleTypeOther;
            private Common.GiveawayTypes raffleType;
            private bool subOnly = false;
            private bool followerOnly = false;
            private Filters raffleFilter;
            private List<Objects.PreviousRaffleWinner> previousWinners;
            private List<string> blockedViewers;
            private int exGamesCount = 0;

            public int ExGamesCount { get { return exGamesCount; } }
            public int Raffle_Length { get { return raffleLength; } }
            public int Raffle_Claim_Length { get { return raffleClaimLength; } }
            public int Raffle_Minimum_Entries { get { return raffleMinimumEntries; } }
            public string Raffle_Type_Other { get { return raffleTypeOther; } }
            public int Raffle_Filter_Amount { get { return raffleFilterAmount; } }
            public int Raffle_Steam_ID { get { return raffleSteamID; } }
            public int Soundbyte_Count { get { return raffleSoundbyteCount; } }
            public Common.GiveawayTypes Raffle_Type { get { return raffleType; } }
            public string Raffle_Donator { get { return raffleDonator; } }
            public string Raffle_Name { get { return raffleName; } }
            public string Raffle_Linker { get { return raffleLinker; } }
            public string Raffle_Author { get { return raffleAuthor; } }
            public Filters Raffle_Filter { get { return raffleFilter; } }
            public bool Sub_Only { get { return subOnly; } }
            public bool Follower_Only { get { return followerOnly; } }
            public List<Objects.PreviousRaffleWinner> Previous_Winners { get { return previousWinners; } }
            public List<string> Blocked_Viewers { get { return blockedViewers; } }

            public RaffleProperties(JToken giveawayProperties)
            {
                raffleLength = int.Parse(giveawayProperties.SelectToken("raffle_length").ToString());
                if (raffleLength < 3)
                    raffleLength = 3;
                raffleClaimLength = int.Parse(giveawayProperties.SelectToken("raffle_claim_length").ToString());
                if (raffleClaimLength < 1)
                    raffleClaimLength = 1;
                raffleMinimumEntries = int.Parse(giveawayProperties.SelectToken("raffle_min_entries").ToString());
                raffleFilterAmount = int.Parse(giveawayProperties.SelectToken("raffle_filter_amount").ToString());
                raffleSteamID = int.Parse(giveawayProperties.SelectToken("raffle_steam_id").ToString());
                raffleSoundbyteCount = int.Parse(giveawayProperties.SelectToken("raffle_soundbyte_count").ToString());
                raffleDonator = giveawayProperties.SelectToken("raffle_donator").ToString();
                raffleAuthor = giveawayProperties.SelectToken("raffle_author").ToString();
                raffleName = giveawayProperties.SelectToken("raffle_name").ToString();
                raffleLinker = giveawayProperties.SelectToken("raffle_linker").ToString();
                if (giveawayProperties.SelectToken("raffle_sub_only").ToString() == "true")
                    subOnly = true;
                if (giveawayProperties.SelectToken("raffle_follower_only").ToString() == "true")
                    followerOnly = true;
                previousWinners = WebCalls.downloadPreviousWinners().Result;
                blockedViewers = downloadBlockedViewers(raffleDonator).Result;

                switch (giveawayProperties.SelectToken("raffle_type").ToString())
                {
                    case "exgames":
                        raffleType = Common.GiveawayTypes.EXGAMES;
                        exGamesCount = WebCalls.downloadExGamesCount().Result;
                        break;
                    case "steam_trade":
                        raffleType = Common.GiveawayTypes.STEAMTRADE;
                        break;
                    case "steam_gift":
                        raffleType = Common.GiveawayTypes.STEAMGIFT;
                        break;
                    case "steam_code":
                        raffleType = Common.GiveawayTypes.STEAMCODE;
                        break;
                    case "origin_code":
                        raffleType = Common.GiveawayTypes.ORIGINCODE;
                        break;
                    case "humblebundle":
                        raffleType = Common.GiveawayTypes.HUMBLEBUNDLE;
                        break;
                    case "code":
                        raffleType = Common.GiveawayTypes.SERIALCODE;
                        break;
                    case "logitech":
                        raffleType = Common.GiveawayTypes.LOGITECH;
                        break;
                    case "soundbyte":
                        raffleType = Common.GiveawayTypes.SOUND_BYTES;
                        break;
                    case "other":
                        raffleType = Common.GiveawayTypes.OTHER;
                        raffleTypeOther = giveawayProperties.SelectToken("raffle_type_other").ToString();
                        break;
                    default:
                        Common.ChatClient.sendMessage(string.Format("ERROR: Raffle type '' is not valid and KrakenBot2 cannot proceed [Fatal] [Restarting]", giveawayProperties.SelectToken("raffle_type").ToString()));
                        System.Diagnostics.Process.Start(Environment.CurrentDirectory + "\\KrakenBot2.exe");
                        Environment.Exit(0);
                        break;
                }
                switch(giveawayProperties.SelectToken("raffle_filter").ToString())
                {
                    case "doubloons":
                        raffleFilter = Filters.DOUBLOONS;
                        break;
                    case "minutes":
                        raffleFilter = Filters.MINUTES;
                        break;
                    case "none":
                        raffleFilter = Filters.NONE;
                        break;
                }
            }
            //Downloads previous blocked viewers (for whatever reason)
            private async Task<List<string>> downloadBlockedViewers(string donator)
            {
                return await WebCalls.downloadBlockedViewers(donator);
            }
        }
    }
}
