using System.Timers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace KrakenBot2
{
    //This class serves to implement multihost and host rotating functionality
    public class Multihost
    {
        private enum StartType
        {
            MANUAL = 1,
            SET_HOST = 2,
            RANDOM = 3
        }
        private Timer rotator = new Timer(60000);
        private int curMinute = 0;
        private int defaultMinuteLimit = 30;
        private StartType startType = StartType.RANDOM;
        private string setHostName;
        private List<Host> hosts;
        private Host currentHost;

        List<UserExtend> extends = new List<UserExtend>();
        private static int extendsAllowed = 2;
        private static int extendDuration = 10;
        private static string multihostInfo = string.Format("Use !extend to extend the hosting duration by {0} minutes.  Use !remaining to see how many minutes remain in the current host.", extendDuration);

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
                    Common.ChatClient.sendMessage(string.Format("Host start type '{0}' is not valid. Defaulting to random.", multihostProperties.SelectToken("start_type").ToString()), Common.DryRun);
                    return;
            }
            hosts = WebCalls.downloadMultihostStreamers().Result;
            rotator.Elapsed += rotatorTick;
        }

        private void rotatorTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (curMinute == defaultMinuteLimit)
                nextHost();
            else
                curMinute++;
        }

        public void refreshHostsList()
        {
            hosts = WebCalls.downloadMultihostStreamers().Result;
        }

        public void start()
        {
            if (firstHost())
                rotator.Start();
        }

        public void stop()
        {
            rotator.Stop();
            unhostStreamer();
            Common.ChatClient.sendMessage("Multihost disabled.", Common.DryRun);
        }

        public bool guess()
        {
            if (currentHost == null)
            {
                Common.ChatClient.sendMessage("Cannot guess the host of a manually assigned host, or a host that does not exist in the multihost list.");
                return false;
            }
            if (TwitchLib.TwitchAPI.broadcasterOnline(currentHost.Streamer).Result)
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
                                Common.ChatClient.sendMessage(string.Format("The next host will be {0}. {1}", onlineHosts[0].Streamer, onlineHosts[1].Information), Common.DryRun);
                            else
                                Common.ChatClient.sendMessage(string.Format("The next host will be {0}.", onlineHosts[0].Streamer), Common.DryRun);
                        } else
                        {
                            if (onlineHosts[curIndex + 1].Information != "")
                                Common.ChatClient.sendMessage(string.Format("The next host will be {0}. {1}", onlineHosts[curIndex + 1].Streamer, onlineHosts[curIndex + 1].Information), Common.DryRun);
                            else
                                Common.ChatClient.sendMessage(string.Format("The next host will be {0}.", onlineHosts[curIndex + 1].Streamer), Common.DryRun);
                        }
                        return true;
                    }
                    curIndex++;
                }
                Common.ChatClient.sendMessage("Current online host was not found in multihost list.  Next host will be random online multihost streamer.", Common.DryRun);
                return false;
            } else
            {
                Common.ChatClient.sendMessage("The current host is not online.  The next host will be a random online multihost streamer.", Common.DryRun);
                return false;
            }
        }

        private bool firstHost()
        {
            switch (startType)
            {
                case StartType.RANDOM:
                    Common.ChatClient.sendMessage("Multihost started. Discovering a random online multihost streamer...", Common.DryRun);
                    List<Host> onlineHosts = WebCalls.getOnlineMultihostStreamers(hosts).Result;
                    if (onlineHosts.Count != 0)
                    {
                        currentHost = onlineHosts[new Random().Next(0, onlineHosts.Count - 1)];
                        if (currentHost.Information != "")
                            Common.ChatClient.sendMessage(string.Format("We'll kick things off with '{0}'. {1}", currentHost.Streamer, currentHost.Information), Common.DryRun);
                        else
                            Common.ChatClient.sendMessage(string.Format("We'll kick things off with '{0}'.", currentHost.Streamer), Common.DryRun);
                        Common.ChatClient.sendMessage(multihostInfo);
                    } else
                    {
                        Common.ChatClient.sendMessage("No online hosts detected.  Multihost random function failed.", Common.DryRun);
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
                                Common.ChatClient.sendMessage(string.Format("Multihost started with '{0}'. {1}", streamer.Streamer, streamer.Information), Common.DryRun);
                            else
                                Common.ChatClient.sendMessage(string.Format("Multihost started with '{0}'.", streamer.Streamer), Common.DryRun);
                            return true;
                        }
                    }
                    Host newHost = new Host(setHostName, "");
                    hosts.Add(newHost);
                    currentHost = newHost;
                    Common.ChatClient.sendMessage(string.Format("Multihost started with '{0}'.", newHost.Streamer), Common.DryRun);
                    hostStreamer(newHost, true);
                    Common.ChatClient.sendMessage(multihostInfo);
                    break;
                case StartType.MANUAL:
                    Common.ChatClient.sendMessage(string.Format("Multihost started, remaining on the current host! Use !extend to extend the host by {0} minutes. Use !remaining to see how many minutes remain in the host! Use !checkhost to rotate offline host.", extendDuration), Common.DryRun);
                    break;
            }
            return true;
        }

        //Rotates to next ONLINE host in multihost list
        private bool nextHost()
        {
            if (TwitchLib.TwitchAPI.broadcasterOnline("burkeblack").Result)
            {
                Common.ChatClient.sendMessage("[Multihost] Burke detected as being online.  Multihost stopped.");
                rotator.Stop();
                return false;
            }
            Host nextHost;
            refreshHostsList();
            List<Host> onlineHosts = WebCalls.getOnlineMultihostStreamers(hosts).Result;
            if (onlineHosts.Count != 0)
            {
                if (currentHost != null && TwitchLib.TwitchAPI.broadcasterOnline(currentHost.Streamer).Result)
                {
                    int index = 0;
                    int currentHostIndex = 0;
                    foreach (Host host in onlineHosts)
                    {
                        if (currentHost.Streamer == host.Streamer)
                            currentHostIndex = index;
                        index++;
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
                    Common.ChatClient.sendMessage(string.Format("Next host: {0}. {1}", nextHost.Streamer, nextHost.Information), Common.DryRun);
                else
                    Common.ChatClient.sendMessage(string.Format("Next host: {0}.", nextHost.Streamer), Common.DryRun);
                Common.ChatClient.sendMessage(string.Format("Use !extend to extend the host by {0} minutes. Use !remaining to see how many minutes remain in the host! Use !checkhost to rotate offline host.", extendDuration), Common.DryRun);
                curMinute = 0;
                if (currentHost != null && nextHost.Streamer.ToLower() != currentHost.Streamer.ToLower())
                    hostStreamer(nextHost);
                currentHost = nextHost;
                return true;
            } else
            {
                Common.ChatClient.sendMessage("There are no multihost channels online at the moment.  Will try again on next rotation.", Common.DryRun);
                curMinute = 0;
                return false;
            }
        }

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
                            curMinute -= extendDuration;
                            int extendsRemaining = extend.useExtend();
                            if (extendsRemaining > 0)
                                Common.ChatClient.sendMessage(string.Format("{0} has extended {1}'s host by {2} minutes! They have {3} remaining host extension(s).",
                                    extend.Username, currentHost.Streamer, extendDuration, extend.RemainingExtends), Common.DryRun);
                            else
                                Common.ChatClient.sendMessage(string.Format("{0} has extended {1}'s host by {2} minutes! They have no remaining host extensions.",
                                    extend.Username, currentHost.Streamer, extend.ExtendBy), Common.DryRun);
                            return true;
                        } else
                        {
                            Common.ChatClient.sendMessage(string.Format("You have used all of your allotted extends for this host, {0}.", extend.Username), Common.DryRun);
                            return false;
                        }
                    }
                }
                //Viewer has not used extend, create new listing for them
                curMinute -= extendDuration;
                UserExtend newExtend = new UserExtend(username, extendsAllowed, extendDuration);
                extends.Add(newExtend);
                Common.ChatClient.sendMessage(string.Format("{0} has extended {1}'s host by {2} minutes! They have {3} remaining host extension(s).",
                                    newExtend.Username, currentHost.Streamer, extendDuration, newExtend.RemainingExtends), Common.DryRun);
                return true;
            } else
            {
                Common.ChatClient.sendMessage("The currently hosted streamer cannot be extended.", Common.DryRun);
                return false;
            }
        }

        public void handleHostOfflineDetected()
        {
            if(currentHost != null)
            {
                Common.ChatClient.sendMessage(string.Format("It looks like {0} just went offline! Rotating host!"));
                nextHost();
            } else
            {
                Common.ChatClient.sendMessage(string.Format("The hosted streamer just went offline! Rotating host!"));
                nextHost();
            }
        }

        public void checkHost()
        {
            if (currentHost != null)
            {
                if (TwitchLib.TwitchAPI.broadcasterOnline(currentHost.Streamer).Result)
                {
                    Common.ChatClient.sendMessage(string.Format("The current host ({0}) is currently online.  Please only use this command to rotate offline hosts.", currentHost.Streamer), Common.DryRun);
                }
                else
                {
                    Common.ChatClient.sendMessage(string.Format("The current host ({0}) appears to be offline.  Rotating to new host.", currentHost.Streamer), Common.DryRun);
                    nextHost();
                }
            } else
            {
                Common.ChatClient.sendMessage("The currently hosted streamer cannot be checked.", Common.DryRun);
            }

        }

        public void remaining()
        {
            if (defaultMinuteLimit - curMinute == 1)
                Common.ChatClient.sendMessage(string.Format("There is currently one minute remaining in {0}'s host. Use !next to get a guess of which streamer will be hosted next.", currentHost.Streamer), Common.DryRun);
            else
                if(currentHost == null)
                    Common.ChatClient.sendMessage(string.Format("There are currently {0} minutes remaining in the current host. Use !next to get a guess of which streamer will be hosted next.", defaultMinuteLimit - curMinute), Common.DryRun);
                else
                    Common.ChatClient.sendMessage(string.Format("There are currently {0} minutes remaining {1}'s host. Use !next to get a guess of which streamer will be hosted next.", defaultMinuteLimit - curMinute, currentHost.Streamer), Common.DryRun);
        }

        private void hostStreamer(Host streamer, bool firstHost = false)
        {
            if(!firstHost)
                Common.ChatClient.sendMessage("/unhost");
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine(string.Format("Host command sent: /host {0}", streamer.Streamer));
            Common.ChatClient.sendMessage(string.Format("/host {0}", streamer.Streamer));
        }

        private void unhostStreamer()
        {
            Common.ChatClient.sendMessage("/unhost");
        }

        class UserExtend
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

            public UserExtend(string username, int allowedExtends, int extendBy)
            {
                this.username = username;
                this.allowedExtends = allowedExtends;
                this.extendBy = extendBy;
            }

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
    public class Host
    {
        private string streamer;
        private string information;

        public string Streamer { get { return streamer; } }
        public string Information { get { return information; } }

        public Host(string streamer, string information)
        {
            this.streamer = streamer;
            this.information = information;
        }
    }
}
