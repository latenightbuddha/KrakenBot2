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
        private string channel;
        private Timer raidTimer = new Timer(60000);
        private int raidLength = 5;
        private List<string> participants = new List<string>();
        private string[] participantIdentifiers = { "r)", "burkeship", "burkebooty", "burkefire", "burkeboom", "ahoy", "booty", "burkeboy", "burkepew", "burkeomg", "burkeahoy",
                                                        "burkelove", "burkeepic", "burkeflag"};

        public string Channel { get { return channel; } }

        public RaidInstance(string channel)
        {
            this.channel = channel;
            raidTimer.Elapsed += raidTimerTick;
            Common.RaidClient = new TwitchLib.TwitchChatClient(channel, new TwitchLib.ConnectionCredentials(TwitchLib.ConnectionCredentials.ClientType.CHAT, new TwitchLib.TwitchIpAndPort(channel, true), "the_kraken_bot", Properties.Settings.Default.krakenOAuth));
            Common.RaidClient.NewChatMessage += Events.raidOnMessage;
            Common.RaidClient.connect();
        }

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

        private int minutesCompleted = 0;
        private void raidTimerTick(object sender, ElapsedEventArgs e)
        {
            if (minutesCompleted == raidLength)
                postRaid();
            else
                minutesCompleted++;
        }

        private void postRaid()
        {
            if (participants.Count != 0)
            {
                Common.RaidClient.disconnect();
                Common.ChatClient.sendMessage(string.Format("The raid has ended! In total, there were {0} participants in the raid, with {1} leading the charge! Your doubloon counts will be updated shortly to reflect your raid participation!", participants.Count, participants[0]), Common.DryRun);
                WebCalls.distibuteDoubloons(participants.Count);
                Common.ChatClient.sendMessage("[Auto] Doubloon counts updated successfully!");
            }  else {
                Common.ChatClient.sendMessage("No one participated in the raid! :(", Common.DryRun);
            }
                
        }
    }
}
