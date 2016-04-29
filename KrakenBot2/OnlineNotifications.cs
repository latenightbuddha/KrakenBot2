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
        // Configurable variables
        private Timer burkeOnlineTimer = new Timer(60000);
        private string[] greetings = { "Bonjour!", "Hola!", "Guten tag!", "Ciao!", "Namaste!", "Salaam!", "Goedendag!", "Szia!", "Dobar dan!", "God dag!", "Good day!" };

        private List<string> usersToNotify;
        private bool currentlyOnline = false;

        // Constructor for OnlineNotifications
        public OnlineNotifications()
        {
            currentlyOnline = Common.StreamRefresher.isOnline();
            usersToNotify = new List<String>(); //WebCalls.downloadUsersToNotify().Result;
            burkeOnlineTimer.Elapsed += burkeOnlineTimerTick;
            burkeOnlineTimer.Start();
        }

        // OnlineNotifications timer tick event
        private void burkeOnlineTimerTick(object sender, ElapsedEventArgs e)
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
                    Common.AhoyRewarder.restartActive();
                }
                    
            }
        }

        // Public method to remove user from notifyMe list
        public void removeMe(string username)
        {
            WebCalls.removeNotifyUser(username);
            usersToNotify.Remove(username);
            if (Common.WhisperClient != null)
                Common.WhisperClient.sendWhisper(username, "You will no longer be notified when Burke goes live. :( To be notified again, whisper !notifyme", Common.DryRun);
        }

        // Public method to add user to notifyMe list
        public void notifyMe(string username)
        {
            WebCalls.addNotifyUser(username);
            usersToNotify.Add(username);
            if (Common.WhisperClient != null)
                Common.WhisperClient.sendWhisper(username, "You will now be notified via a whisper when Burke goes live! burkeEpic To remove yourself from this list, whisper !removeme", Common.DryRun);
        }

        // Method to send message in discord and fire all whispers
        private void fireAllWhisperNotifications()
        {
            TwitchLib.TwitchChannel channel = Common.StreamRefresher.Stream.Channel;
            if (Common.DiscordClient != null)
                Common.relay(string.Format("{0} Hey guys! Burke just went live playing: {1}, title: {2}. Channel: {3}.", greetings[new Random().Next(0,greetings.Length)], channel.Game, channel.Status, "http://twitch.tv/burkeblack"));
            foreach(string user in usersToNotify)
            {
                if (Common.WhisperClient != null)
                    Common.WhisperClient.sendWhisper(user, string.Format("Burke just went live playing: {0}, title: {1}. Channel: {2}. !removeme to stop receiving notifications.", channel.Game, channel.Status,  "http://twitch.tv/burkeblack"), Common.DryRun);
                System.Threading.Thread.Sleep(2000);
            }
        }
    }
}
