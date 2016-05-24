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
        private string[] boarderIdentifiers = { "r)" };
        private string[] gunnerIdentifiers = { "burkeship", "burkebooty", "burkefire", "burkeboom", "ahoy", "booty", "burkeomg", "burkeahoy",
                                                        "burkelove", "burkeepic", "burkeflag", "burkeevil", "burkemug"};
        private Timer raidTimer = new Timer(60000);
        private int raidLength = 5;

        private string channel;
        private List<string> boarders = new List<string>(); // Users that use R)
        private List<string> gunners = new List<string>(); // Users that use channel emotes
        private List<string> participants = new List<string>();
        
        public string Channel { get { return channel; } }

        // RaidInstance constructor accepts channel param
        public RaidInstance(string channel)
        {
            this.channel = channel;
            raidTimer.Elapsed += raidTimerTick;
            Common.RaidClient = new TwitchLib.TwitchChatClient(channel, new TwitchLib.ConnectionCredentials(TwitchLib.ConnectionCredentials.ClientType.Chat, new TwitchLib.TwitchIpAndPort(channel, true), "the_kraken_bot", Properties.Settings.Default.krakenOAuth));
            Common.RaidClient.OnMessageReceived += Events.raidOnMessage;
            Common.RaidClient.Connect();
        }

        // Handles raid chat event
        public void handleMessage(TwitchLib.TwitchChatClient.OnMessageReceivedArgs message)
        {
            if(raidTimer.Enabled)
            {
                foreach(string word in message.ChatMessage.Message.ToLower().Split(' '))
                {
                    foreach(string gunnerWord in gunnerIdentifiers)
                    {
                        if (word == gunnerWord && !gunners.Contains(message.ChatMessage.Username.ToLower()) && message.ChatMessage.Username.ToLower() != "the_kraken_bot")
                            gunners.Add(message.ChatMessage.Username.ToLower());
                        if (word == gunnerWord && !participants.Contains(message.ChatMessage.Username.ToLower()) && message.ChatMessage.Username.ToLower() != "the_kraken_bot")
                            participants.Add(message.ChatMessage.Username.ToLower());
                    }
                    foreach(string boarderWord in boarderIdentifiers)
                    {
                        if (word == boarderWord && !boarders.Contains(message.ChatMessage.Username.ToLower()) && message.ChatMessage.Username.ToLower() != "the_kraken_bot")
                            boarders.Add(message.ChatMessage.Username.ToLower());
                        if (word == boarderWord && !participants.Contains(message.ChatMessage.Username.ToLower()) && message.ChatMessage.Username.ToLower() != "the_kraken_bot")
                            participants.Add(message.ChatMessage.Username.ToLower());
                    }
                }
            } else
            {
                if(message.ChatMessage.Username.ToLower() == "burkeblack")
                {
                    raidTimer.Start();
                    Common.RaidClient.SendMessage("burkeFlag burkeFlag The Black Crew, ATTACK!!!! burkeFlag burkeFlag", Common.DryRun);
                    Common.ChatClient.SendMessage(string.Format("burkeFlag burkeFlag The attack has begun on {0}, get in there! http://twitch.tv/{0}", message.ChatMessage.Channel), Common.DryRun);
                }
            }
        }

        // Length raid client has been connected to raided channel
        private int minutesCompleted = 0;

        // Raid Timer tick event
        private void raidTimerTick(object sender, ElapsedEventArgs e)
        {
            if (minutesCompleted == raidLength)
            {
                raidTimer.Stop();
                postRaid();
            }
            else
            {
                minutesCompleted++;
            }
                
        }

        // Handles post raid messages and doubloon distrobution
        private bool postRaidFired = false;
        private void postRaid()
        {
            if (postRaidFired)
                return;
            else
                postRaidFired = true;
            if (participants.Count != 0)
            {
                Common.RaidClient.Disconnect();
                if(boarders.Count  == 0 || gunners.Count == 0)
                    Common.ChatClient.SendMessage(string.Format("The raid has ended! There were {0} ( R) ) boarders and {1} ( burkeShip burkeFire burkeFire ) gunners!! In total, there were {2} participants in this raid, with boarder {3} and gunner {4} leading the charge! Your doubloon counts will be updated shortly!", boarders.Count, gunners.Count, participants.Count, boarders[0], gunners[0]), Common.DryRun);
                else
                    Common.ChatClient.SendMessage(string.Format("The raid has ended! There were {0} ( R) ) boarders and {1} ( burkeShip burkeFire burkeFire ) gunners!! In total, there were {2} participants in this raid! Your doubloon counts will be updated shortly!", boarders.Count, gunners.Count, participants.Count), Common.DryRun);

                if (WebCalls.distibuteDoubloons(participants.Count).Result)
                    Common.ChatClient.SendMessage("[Auto] Doubloon counts updated successfully!");
                else
                    Common.ChatClient.SendMessage("[Auto] Doubloon counts FAILED TO UPDATE");
            }  else {
                Common.ChatClient.SendMessage("No one participated in the raid! :(", Common.DryRun);
            }
                
        }
    }
}
