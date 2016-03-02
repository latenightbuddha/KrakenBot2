using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2
{
    public class AhoyRewarder
    {
        // Ahoy rewards
        private int firstReward = 25;
        private int secondReward = 15;
        private int thirdReward = 5;

        ahoyReward first, second, third;
        private bool connectedMsgReceived = false;

        // AhoyRewarder constructor
        public AhoyRewarder() { }

        // Resets AhoyRewarder
        public void restartActive()
        {
            first = null;
            second = null;
            third = null;
            connectedMsgReceived = false;
        }

        // Determines if any rewards are available
        public bool isActive()
        {
            return (first == null || second == null || third == null);
        }

        // Process message from event
        public void processMessage(TwitchLib.TwitchChatClient.NewChatMessageArgs e)
        {
            if (connectedMsgReceived)
            {
                if(e.ChatMessage.Message.ToLower().Contains("ahoy"))
                {
                    if(!isFraud(e.ChatMessage.Username)) {
                        if (first == null)
                        {
                            first = new ahoyReward(e.ChatMessage.Username, firstReward);
                            WebCalls.addDoubloons(first.Username, first.Reward);
                            Common.ChatClient.sendMessage(string.Format("/me rewarded {0} some doubloons ({1}) for the first Ahoy message! [auto] ", first.Username, first.Reward));
                            Common.DiscordClient.SendMessageToChannel(String.Format("[ahoy] First ahoy reward ({0} doubloons) awarded to: {1}", first.Reward, first.Username), Common.DiscordClient.GetChannelByName("kraken-relay"));
                        }
                        else if (second == null)
                        {
                            second = new ahoyReward(e.ChatMessage.Username, secondReward);
                            WebCalls.addDoubloons(second.Username, second.Reward);
                            Common.ChatClient.sendMessage(string.Format("/me rewarded {0} some doubloons ({1}) for the second Ahoy message! [auto] ", second.Username, second.Reward));
                            Common.DiscordClient.SendMessageToChannel(String.Format("[ahoy] Second ahoy reward ({0} doubloons) awarded to: {1}", second.Reward, second.Username), Common.DiscordClient.GetChannelByName("kraken-relay"));
                        }
                        else
                        {
                            third = new ahoyReward(e.ChatMessage.Username, thirdReward);
                            WebCalls.addDoubloons(third.Username, third.Reward);
                            Common.ChatClient.sendMessage(string.Format("/me rewarded {0} some doubloons ({1}) for the third Ahoy message! [auto] ", third.Username, third.Reward));
                            connectedMsgReceived = false;
                            Common.DiscordClient.SendMessageToChannel(String.Format("[ahoy] Third ahoy reward ({0} doubloons) awarded to: {1}", third.Reward, third.Username), Common.DiscordClient.GetChannelByName("kraken-relay"));
                        }
                    }
                }
            } else
            {
                if (e.ChatMessage.Username.ToLower() == "burke_listener" && e.ChatMessage.Message.ToLower().Contains("connected!") && !e.ChatMessage.Message.ToLower().Contains("disconnect"))
                    connectedMsgReceived = true;
            }
        }

        // Determine if someone has already been rewarded
        private bool isFraud(string username)
        {
            if (first != null)
                if (first.Username.ToLower() == username.ToLower())
                    return true;
            if (second != null)
                if (second.Username.ToLower() == username.ToLower())
                    return true;
            if (third != null)
                if (third.Username.ToLower() == username.ToLower())
                    return true;
            return false;
        }

        // Class for ahoy reward
        private class ahoyReward
        {
            private string username;
            private int reward;

            public string Username { get { return username; } }
            public int Reward { get { return reward; } }

            // ahoyReward constructor
            public ahoyReward(string username, int reward)
            {
                this.username = username;
                this.reward = reward;
            }
        }
    }
}
