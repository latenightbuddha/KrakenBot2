using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KrakenBot2.Objects
{
    public class PreviousRaffleWinner
    {
        private string username;
        private Common.GiveawayTypes giveawayType;
        private bool affectWinLimit = true;

        public string Username { get { return username; } }
        public Common.GiveawayTypes GiveawayType { get { return giveawayType; } }
        public bool AffectWinLimit { get { return affectWinLimit; } }

        public PreviousRaffleWinner(JToken previousWinnerData)
        {
            username = previousWinnerData.SelectToken("username").ToString();
            if (previousWinnerData.SelectToken("affect_win_limit").ToString() == "False")
                affectWinLimit = false;
            switch(previousWinnerData.SelectToken("giveaway_type").ToString())
            {
                case "exgames":
                    giveawayType = Common.GiveawayTypes.EXGAMES;
                    break;
                case "steam_trade":
                    giveawayType = Common.GiveawayTypes.STEAMTRADE;
                    break;
                case "steam_gift":
                    giveawayType = Common.GiveawayTypes.STEAMGIFT;
                    break;
                case "steam_code":
                    giveawayType = Common.GiveawayTypes.STEAMCODE;
                    break;
                case "origin_code":
                    giveawayType = Common.GiveawayTypes.ORIGINCODE;
                    break;
                case "humblebundle":
                    giveawayType = Common.GiveawayTypes.HUMBLEBUNDLE;
                    break;
                case "code":
                    giveawayType = Common.GiveawayTypes.SERIALCODE;
                    break;
                case "logitech":
                    giveawayType = Common.GiveawayTypes.LOGITECH;
                    break;
                case "soundbyte":
                    giveawayType = Common.GiveawayTypes.SOUND_BYTES;
                    break;
                case "other":
                    giveawayType = Common.GiveawayTypes.OTHER;
                    break;
                default:
                    giveawayType = Common.GiveawayTypes.OTHER;
                    break;
            }
        }
    }
}
