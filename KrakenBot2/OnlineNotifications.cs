using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KrakenBot2
{
    //Class will serve to implement user whisper notifications when burkeblack goes live
    public class OnlineNotifications
    {
        private Timer burkeOnlineTimer = new Timer(60000);
        private List<string> usersToNotify;
        private bool currentlyOnline = false;

        public OnlineNotifications()
        {
            currentlyOnline = Common.StreamRefresher.isOnline();
            usersToNotify = WebCalls.downloadUsersToNotify().Result;
            burkeOnlineTimer.Elapsed += burkeOnlineTimerTick;
            burkeOnlineTimer.Start();
        }

        private void burkeOnlineTimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Common.StreamRefresher.isOnline())
            {
                if(!currentlyOnline)
                {
                    currentlyOnline = true;
                    Common.DoubloonDistributor.forceOnline();
                    fireAllWhisperNotifications();
                }
            } else
            {
                if (currentlyOnline)
                {
                    currentlyOnline = false;
                    Common.DoubloonDistributor.forceOffline();
                }
                    
            }
        }

        public void removeMe(string username)
        {
            WebCalls.removeNotifyUser(username);
            usersToNotify.Remove(username);
            Common.WhisperClient.sendWhisper(username, "You will no longer be notified when Burke goes live. :( To be notified again, whisper !notifyme", Common.DryRun);
        }

        public void notifyMe(string username)
        {
            WebCalls.addNotifyUser(username);
            usersToNotify.Add(username);
            Common.WhisperClient.sendWhisper(username, "You will now be notified via a whisper when Burke goes live! burkeEpic To remove yourself from this list, whisper !removeme", Common.DryRun);
        }

        private void fireAllWhisperNotifications()
        {
            TwitchLib.TwitchChannel channel = Common.StreamRefresher.Stream.Channel;
            foreach(string user in usersToNotify)
            {
                Common.WhisperClient.sendWhisper(user, string.Format("Burke just went live playing: {0}, title: {1}. Channel: {2}. !removeme to stop receiving notifications.", channel.Game, channel.Status,  "http://twitch.tv/burkeblack"), Common.DryRun);
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
