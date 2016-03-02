using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KrakenBot2
{
    //These settings are not currently implemented
    public class ServerSettings
    {
        private int capsProtectionChars, spamProtectionChars, emoteSpam;
        private int onlineDoubloonsMins, onlineDoubloonsAmount;
        private int offlineDoubloonsMins, offlineDoubloonsAmount;
        private int automatedGiveawayMins;

        public int Caps_Protection_Chars { get { return capsProtectionChars; } }
        public int Spam_Protection_Chars { get { return spamProtectionChars; } }
        public int Emote_Spam { get { return emoteSpam; } }
        public int Online_Doubloons_Mins { get { return onlineDoubloonsMins; } }
        public int Online_Doubloons_Amount { get { return onlineDoubloonsAmount; } }
        public int Offline_Doubloons_Mins { get { return offlineDoubloonsMins; } }
        public int Offline_Doubloons_Amount { get { return offlineDoubloonsAmount; } }
        public int Automated_Giveaway_Mins { get { return automatedGiveawayMins; } }

        // Constructor for ServerSettings accepts JSON data from API
        public ServerSettings(JToken data)
        {
            capsProtectionChars = int.Parse(data.SelectToken("capsProtectionChars").ToString());
            spamProtectionChars = int.Parse(data.SelectToken("spamProtectionChars").ToString());
            emoteSpam = int.Parse(data.SelectToken("emoteSpam").ToString());
            onlineDoubloonsMins = int.Parse(data.SelectToken("onlineDoubloonsMins").ToString());
            onlineDoubloonsAmount = int.Parse(data.SelectToken("onlineDoubloonsAmount").ToString());
            offlineDoubloonsMins = int.Parse(data.SelectToken("offlineDoubloonsMins").ToString());
            offlineDoubloonsAmount = int.Parse(data.SelectToken("offlineDoubloonsAmount").ToString());
            automatedGiveawayMins = int.Parse(data.SelectToken("automatedGiveawayMinutes").ToString());
        }
    }
}
