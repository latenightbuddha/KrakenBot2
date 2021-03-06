﻿using System.Timers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace KrakenBot2
{
    //This class serves to implement multihost and host rotating functionality
    public class Multihost
    {
        // Configurable variables
        private int defaultMinuteLimit = 30;
        private static int extendsAllowed = 2;
        private static int extendDuration = 10;
        private static string multihostInfo = string.Format("Use !extend to extend the hosting duration by {0} minutes.  Use !remaining to see how many minutes remain in the current host.", extendDuration);

        private enum StartType
        {
            MANUAL = 1,
            SET_HOST = 2,
            RANDOM = 3
        }
        private Timer rotator = new Timer(60000);
        private int curMinute = 0;
        private StartType startType = StartType.RANDOM;
        private string setHostName;
        private List<Host> hosts;
        private Host currentHost;
        List<UserExtend> extends = new List<UserExtend>();

        // Multihost constructor using JSON data from API
        public Multihost(JToken multihostProperties)
        {
            switch (multihostProperties.SelectToken("start_type").ToString())
            {
                case "manual":
                    startType = StartType.MANUAL;
                    break;
                case "set_host":
                    startType = StartType.SET_HOST;
                    setHostName = multihostProperties.SelectToken("set_host_name").ToString();
                    break;
                case "random":
                    startType = StartType.RANDOM;
                    break;
                default:
                    Common.ChatClient.SendMessage(string.Format("Host start type '{0}' is not valid. Defaulting to random.", multihostProperties.SelectToken("start_type").ToString()), Common.DryRun);
                    return;
            }
            hosts = WebCalls.downloadMultihostStreamers().Result;
            rotator.Elapsed += rotatorTick;
        }

        // Multihost timer tick event
        private void rotatorTick(object sender, ElapsedEventArgs e)
        {
            if (curMinute == defaultMinuteLimit)
                nextHost();
            else
                curMinute++;
        }

        // Public method to refresh multihosts list
        public void refreshHostsList()
        {
            hosts = WebCalls.downloadMultihostStreamers().Result;
        }

        // Public method to start timer
        public void start()
        {
            if (firstHost())
                rotator.Start();
        }

        // Public method to stop timer
        public void stop()
        {
            rotator.Stop();
            Common.ChatClient.SendMessage("/unhost");
            Common.ChatClient.SendMessage("Multihost disabled.", Common.DryRun);
        }

        // Public function to guess next hosted streamer, returns true if successful, false if failed
        public bool guess()
        {
            if (currentHost == null)
            {
                Common.ChatClient.SendMessage("Cannot guess the host of a manually assigned host, or a host that does not exist in the multihost list.");
                return false;
            }
            if (TwitchLib.TwitchApi.BroadcasterOnline(currentHost.Streamer).Result)
            {
                refreshHostsList();
                List<Host> onlineHosts = WebCalls.getOnlineMultihostStreamers(hosts).Result;
                int curIndex = 0;
                foreach (Host host in onlineHosts)
                {
                    if (currentHost.Streamer.ToLower() == host.Streamer.ToLower())
                    {
                        if (curIndex == onlineHosts.Count - 1)
                        {
                            if (onlineHosts[0].Information != "")
                                Common.ChatClient.SendMessage(string.Format("The next host will be {0}. {1}", onlineHosts[0].Streamer, onlineHosts[1].Information), Common.DryRun);
                            else
                                Common.ChatClient.SendMessage(string.Format("The next host will be {0}.", onlineHosts[0].Streamer), Common.DryRun);
                        } else
                        {
                            if (onlineHosts[curIndex + 1].Information != "")
                                Common.ChatClient.SendMessage(string.Format("The next host will be {0}. {1}", onlineHosts[curIndex + 1].Streamer, onlineHosts[curIndex + 1].Information), Common.DryRun);
                            else
                                Common.ChatClient.SendMessage(string.Format("The next host will be {0}.", onlineHosts[curIndex + 1].Streamer), Common.DryRun);
                        }
                        return true;
                    }
                    curIndex++;
                }
                Common.ChatClient.SendMessage("Current online host was not found in multihost list.  Next host will be random online multihost streamer.", Common.DryRun);
                return false;
            } else
            {
                Common.ChatClient.SendMessage("The current host is not online.  The next host will be a random online multihost streamer.", Common.DryRun);
                return false;
            }
        }

        // Function that is run on first host, returns true if successful, false if failed
        private bool firstHost()
        {
            switch (startType)
            {
                case StartType.RANDOM:
                    Common.ChatClient.SendMessage("Multihost started. Discovering a random online multihost streamer...", Common.DryRun);
                    List<Host> onlineHosts = WebCalls.getOnlineMultihostStreamers(hosts).Result;
                    if (onlineHosts.Count != 0)
                    {
                        currentHost = onlineHosts[new Random().Next(0, onlineHosts.Count - 1)];
                        if (currentHost.Information != "")
                            Common.ChatClient.SendMessage(string.Format("We'll kick things off with '{0}'. {1}", currentHost.Streamer, currentHost.Information), Common.DryRun);
                        else
                            Common.ChatClient.SendMessage(string.Format("We'll kick things off with '{0}'.", currentHost.Streamer), Common.DryRun);
                        Common.ChatClient.SendMessage(multihostInfo);
                    } else
                    {
                        Common.ChatClient.SendMessage("No online hosts detected.  Multihost random function failed.", Common.DryRun);
                        return false;
                    }
                    break;
                case StartType.SET_HOST:
                    foreach (Host streamer in hosts)
                    {
                        if (streamer.Streamer.ToLower() == setHostName.ToLower())
                        {
                            currentHost = streamer;
                            if (streamer.Information != "")
                                Common.ChatClient.SendMessage(string.Format("Multihost started with '{0}'. {1}", streamer.Streamer, streamer.Information), Common.DryRun);
                            else
                                Common.ChatClient.SendMessage(string.Format("Multihost started with '{0}'.", streamer.Streamer), Common.DryRun);
                            currentHost = streamer;
                            hostStreamer(streamer);
                            return true;
                        }
                    }
                    Host newHost = new Host(setHostName, "");
                    hosts.Add(newHost);
                    currentHost = newHost;
                    Common.ChatClient.SendMessage(string.Format("Multihost started with '{0}'.", newHost.Streamer), Common.DryRun);
                    hostStreamer(newHost);
                    Common.ChatClient.SendMessage(multihostInfo);
                    break;
                case StartType.MANUAL:
                    Common.ChatClient.SendMessage(string.Format("Multihost started, remaining on the current host! Use !extend to extend the host by {0} minutes. Use !remaining to see how many minutes remain in the host!", extendDuration), Common.DryRun);
                    break;
            }
            return true;
        }

        //Rotates to next ONLINE host in multihost list
        private bool nextHost()
        {
            if (TwitchLib.TwitchApi.BroadcasterOnline("burkeblack").Result)
            {
                Common.ChatClient.SendMessage("[Multihost] Burke detected as being online.  Multihost stopped.");
                rotator.Stop();
                return false;
            }
            Host nextHost;
            refreshHostsList();
            List<Host> onlineHosts = WebCalls.getOnlineMultihostStreamers(hosts).Result;
            if (onlineHosts.Count != 0)
            {
                if (currentHost != null && TwitchLib.TwitchApi.BroadcasterOnline(currentHost.Streamer).Result)
                {
                    int multiIndex = 0;
                    int currentHostIndex = 0;
                    foreach (Host host in onlineHosts)
                    {
                        if (currentHost.Streamer == host.Streamer)
                            currentHostIndex = multiIndex;
                        multiIndex++;
                    }
                    if (currentHostIndex == onlineHosts.Count - 1)
                        nextHost = onlineHosts[0];
                    else
                        nextHost = onlineHosts[currentHostIndex + 1];
                }
                else
                {
                    nextHost = onlineHosts[new Random().Next(0, onlineHosts.Count - 1)];
                }
                if (nextHost.Information != "")
                    Common.ChatClient.SendMessage(string.Format("Next host: {0}. {1}", nextHost.Streamer, nextHost.Information), Common.DryRun);
                else
                    Common.ChatClient.SendMessage(string.Format("Next host: {0}.", nextHost.Streamer), Common.DryRun);
                Common.ChatClient.SendMessage(string.Format("Use !extend to extend the host by {0} minutes. Use !remaining to see how many minutes remain in the host!", extendDuration), Common.DryRun);
                curMinute = 0;
                hostStreamer(nextHost);
                return true;
            } else
            {
                Common.ChatClient.SendMessage("There are no multihost channels online at the moment.  Will try again on next rotation.", Common.DryRun);
                curMinute = 0;
                return false;
            }
        }

        // Handles a viewers attempt to extend the currently hosted streamer
        public bool handleExtend(string username)
        {
            //Check to see if viewer has already used an extend
            if (currentHost != null)
            {
                foreach (UserExtend extend in extends)
                {
                    if (extend.Username.ToLower() == username.ToLower())
                    {
                        if (extend.RemainingExtends > 0)
                        {
                            curMinute -= extend.ExtendBy;
                            int extendsRemaining = extend.useExtend();
                            if (extendsRemaining > 0)
                                Common.ChatClient.SendMessage(string.Format("{0} has extended {1}'s host by {2} minutes! They have {3} remaining host extension(s).",
                                    extend.Username, currentHost.Streamer, extendDuration, extend.RemainingExtends), Common.DryRun);
                            else
                                Common.ChatClient.SendMessage(string.Format("{0} has extended {1}'s host by {2} minutes! They have no remaining host extensions.",
                                    extend.Username, currentHost.Streamer, extend.ExtendBy), Common.DryRun);
                            return true;
                        } else
                        {
                            Common.ChatClient.SendMessage(string.Format("You have used all of your allotted extends for this host, {0}.", extend.Username), Common.DryRun);
                            return false;
                        }
                    }
                }
                //Viewer has not used extend, create new listing for them
                curMinute -= extendDuration;
                UserExtend newExtend = new UserExtend(username, extendsAllowed, extendDuration);
                extends.Add(newExtend);
                Common.ChatClient.SendMessage(string.Format("{0} has extended {1}'s host by {2} minutes! They have {3} remaining host extension(s).",
                                    newExtend.Username, currentHost.Streamer, extendDuration, newExtend.RemainingExtends), Common.DryRun);
                return true;
            } else
            {
                Common.ChatClient.SendMessage("The currently hosted streamer cannot be extended.", Common.DryRun);
                return false;
            }
        }

        // Handle an event in which the currently hosted streamer is determined to have gone offline
        public void handleHostOfflineDetected()
        {
            if(currentHost != null)
            {
                Common.ChatClient.SendMessage(string.Format("It looks like {0} just went offline! Rotating host!", currentHost.Streamer), Common.DryRun);
                nextHost();
            } else
            {
                Common.ChatClient.SendMessage(string.Format("The hosted streamer just went offline! Rotating host!"), Common.DryRun);
                nextHost();
            }
        }

        // Public method that sends chat message indicating time remaining in currently hosted streamer
        public void remaining()
        {
            if (defaultMinuteLimit - curMinute == 1)
                Common.ChatClient.SendMessage(string.Format("There is currently one minute remaining in {0}'s host. Use !next to get a guess of which streamer will be hosted next.", currentHost.Streamer), Common.DryRun);
            else
                if(currentHost == null)
                    Common.ChatClient.SendMessage(string.Format("There are currently {0} minutes remaining in the current host. Use !next to get a guess of which streamer will be hosted next.", defaultMinuteLimit - curMinute), Common.DryRun);
                else
                    Common.ChatClient.SendMessage(string.Format("There are currently {0} minutes remaining {1}'s host. Use !next to get a guess of which streamer will be hosted next.", defaultMinuteLimit - curMinute, currentHost.Streamer), Common.DryRun);
        }

        // Method to initiate host of a streamer
        private void hostStreamer(Host streamer)
        {
            Common.ChatClient.SendMessage(string.Format(".host {0}", streamer.Streamer));
            currentHost = streamer;
            if (defaultMinuteLimit - curMinute < (defaultMinuteLimit - 1))
                curMinute -= defaultMinuteLimit - (defaultMinuteLimit - curMinute);
        }

        // Class that represents properties of a user extend and subsequently limits them
        private class UserExtend
        {
            private string username;
            private int allowedExtends;
            private int usedExtends = 1;
            private int extendBy;

            public string Username { get { return username; } }
            public int AllowedExtends { get { return allowedExtends; } }
            public int UsedExtends { get { return usedExtends; } }
            public int RemainingExtends { get { return allowedExtends - usedExtends; } }
            public int ExtendBy { get { return extendBy; } }

            // UserExtend constructor accepting username, allowedExtends (from vars above), and extendBy (from vars above)
            public UserExtend(string username, int allowedExtends, int extendBy)
            {
                this.username = username;
                this.allowedExtends = allowedExtends;
                this.extendBy = extendBy;
            }

            // Attempts to use an extend
            public int useExtend()
            {
                if(allowedExtends - usedExtends > 0)
                {
                    usedExtends++;
                    return allowedExtends - usedExtends;
                } else
                {
                    return -1;
                }
            }
        }
    }

    // Class that represents the properties of a particular host
    public class Host
    {
        private string streamer;
        private string information;

        public string Streamer { get { return streamer; } }
        public string Information { get { return information; } }

        // Constructor for a Host accepting the streamer name and information (if available)
        public Host(string streamer, string information = "")
        {
            this.streamer = streamer;
            this.information = information;
        }
    }
}
