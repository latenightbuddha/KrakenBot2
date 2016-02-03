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
        private bool ENABLED = false;

        List<TwitchLib.TwitchAPIClasses.TwitchFollower> recentFollowers = new List<TwitchLib.TwitchAPIClasses.TwitchFollower>();
        private Timer followerTimer = new Timer(900000);
        public FollowerTracker()
        {
            if(ENABLED)
            {
                recentFollowers = TwitchLib.TwitchAPI.getTwitchFollowers("burkeblack", 50); 
                followerTimer.Elapsed += followerTimerTick;
                followerTimer.Start();
            }
            
        }

        private void followerTimerTick(object sender, ElapsedEventArgs e)
        {
            List<TwitchLib.TwitchAPIClasses.TwitchFollower> newFollowers = new List<TwitchLib.TwitchAPIClasses.TwitchFollower>();
            List<TwitchLib.TwitchAPIClasses.TwitchFollower> followers = TwitchLib.TwitchAPI.getTwitchFollowers("burkeblack", 50);
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

        private void handleNewFollowers(List<TwitchLib.TwitchAPIClasses.TwitchFollower> newFollowers)
        {
            if(newFollowers.Count == 1)
            {
                Common.ChatClient.sendMessage(string.Format("New follower from the last 15 minutes: {0}", newFollowers[0].User.DisplayName), Common.DryRun);
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
                Common.ChatClient.sendMessage(string.Format("New followers from the last 15 minutes: {0}", msgStr), Common.DryRun);
            }
        }
    }
}
