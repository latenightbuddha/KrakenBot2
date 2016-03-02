using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KrakenBot2
{
    public class RaidInstance
    {
        // Configurable variables
        private string[] participantIdentifiers = { "r)", "burkeship", "burkebooty", "burkefire", "burkeboom", "ahoy", "booty", "burkeomg", "burkeahoy",
                                                        "burkelove", "burkeepic", "burkeflag", "burkeevil", "burkemug"};
        private Timer raidTimer = new Timer(60000);
        private int raidLength = 5;

        private string channel;
        private List<string> participants = new List<string>();
        
        public string Channel { get { return channel; } }

        // RaidInstance constructor accepts channel param
        public RaidInstance(string channel)
        {
            this.channel = channel;
            raidTimer.Elapsed += raidTimerTick;
            Common.RaidClient = new TwitchLib.TwitchChatClient(channel, new TwitchLib.ConnectionCredentials(TwitchLib.ConnectionCredentials.ClientType.CHAT, new TwitchLib.TwitchIpAndPort(channel, true), "the_kraken_bot", Properties.Settings.Default.krakenOAuth));
            Common.RaidClient.NewChatMessage += Events.raidOnMessage;
            Common.RaidClient.connect();
        }

        // Handles raid chat event
        public void handleMessage(TwitchLib.TwitchChatClient.NewChatMessageArgs message)
        {
            if(raidTimer.Enabled)
            {
                foreach(string word in message.ChatMessage.Message.ToLower().Split(' '))
                {
                    foreach(string partWord in participantIdentifiers)
                    {
                        if (word == partWord && !participants.Contains(message.ChatMessage.Username.ToLower()) && message.ChatMessage.Username.ToLower() != "the_kraken_bot")
                            participants.Add(message.ChatMessage.Username.ToLower());
                    }
                }
            } else
            {
                if(message.ChatMessage.Username.ToLower() == "burkeblack")
                {
                    raidTimer.Start();
                    Common.RaidClient.sendMessage("burkeFlag burkeFlag The Black Crew, ATTACK!!!! burkeFlag burkeFlag", Common.DryRun);
                    Common.ChatClient.sendMessage(string.Format("burkeFlag burkeFlag The attack has beguon on {0}, get in there! http://twitch.tv/{0}", message.ChatMessage.Channel), Common.DryRun);
                }
            }
        }

        // Length raid client has been connected to raided channel
        private int minutesCompleted = 0;

        // Raid Timer tick event
        private void raidTimerTick(object sender, ElapsedEventArgs e)
        {
            if (minutesCompleted == raidLength)
                postRaid();
            else
                minutesCompleted++;
        }

        // Handles post raid messages and doubloon distrobution
        private void postRaid()
        {
            if (participants.Count != 0)
            {
                Common.RaidClient.disconnect();
                Common.ChatClient.sendMessage(string.Format("The raid has ended! In total, there were {0} participants in the raid, with {1} leading the charge! Your doubloon counts will be updated shortly to reflect your raid participation!", participants.Count, participants[0]), Common.DryRun);
                if (WebCalls.distibuteDoubloons(participants.Count).Result)
                    Common.ChatClient.sendMessage("[Auto] Doubloon counts updated successfully!");
                else
                    Common.ChatClient.sendMessage("[Auto] Doubloon counts FAILED TO UPDATE");
            }  else {
                Common.ChatClient.sendMessage("No one participated in the raid! :(", Common.DryRun);
            }
                
        }
    }
}
