using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KrakenBot2
{
    public class FollowerTracker
    {
        // Enable/disable follow tracker
        private bool ENABLED = false;

        private List<TwitchLib.TwitchAPIClasses.TwitchFollower> recentFollowers = new List<TwitchLib.TwitchAPIClasses.TwitchFollower>();
        private Timer followerTimer = new Timer(900000);

        // Follow tracker constructor
        public FollowerTracker()
        {
            if(ENABLED)
            {
                // Gets the most recent 50 followers of burkeblack
                recentFollowers = TwitchLib.TwitchApi.GetTwitchFollowers("burkeblack", 50).Result; 
                followerTimer.Elapsed += followerTimerTick;
                followerTimer.Start();
            }
        }

        // Follower timer tick event
        private void followerTimerTick(object sender, ElapsedEventArgs e)
        {
            List<TwitchLib.TwitchAPIClasses.TwitchFollower> newFollowers = new List<TwitchLib.TwitchAPIClasses.TwitchFollower>();
            // Gets the most recent 50 followers of burkeblack
            List<TwitchLib.TwitchAPIClasses.TwitchFollower> followers = TwitchLib.TwitchApi.GetTwitchFollowers("burkeblack", 50).Result;
            foreach(TwitchLib.TwitchAPIClasses.TwitchFollower follower in recentFollowers)
            {
                bool found = false;
                foreach(TwitchLib.TwitchAPIClasses.TwitchFollower newFollow in followers)
                {
                    if (follower.User.Name.ToLower() == newFollow.User.Name.ToLower())
                        found = true;
                }
                if (found == false)
                    newFollowers.Add(follower);
            }
            recentFollowers = followers;
            if (newFollowers.Count > 0)
                handleNewFollowers(newFollowers);
        }

        // Handles new followers by constructing string with all names and sending to chat
        private void handleNewFollowers(List<TwitchLib.TwitchAPIClasses.TwitchFollower> newFollowers)
        {
            if(newFollowers.Count == 1)
            {
                Common.ChatClient.SendMessage(string.Format("New follower from the last 15 minutes: {0}", newFollowers[0].User.DisplayName), Common.DryRun);
            } else
            {
                string msgStr = "";
                foreach (TwitchLib.TwitchAPIClasses.TwitchFollower follower in newFollowers)
                {
                    if (msgStr == "")
                        msgStr = follower.User.DisplayName;
                    else
                        msgStr += ", " + follower.User.DisplayName;
                }
                Common.ChatClient.SendMessage(string.Format("New followers from the last 15 minutes: {0}", msgStr), Common.DryRun);
            }
        }
    }
}
