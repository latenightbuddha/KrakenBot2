using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KrakenBot2.Objects
{
    public class RaffleWin
    {
        private string raffleJsonData;
        private string winner;
        private int winCount;
        private int enterCount;
        private double claimTime;
        private double claimTimeAvg;

        public string Winner { get { return winner; } }
        public int WinCount { get { return winCount; } }
        public int EnterCount { get { return enterCount; } }
        public double WinPercentage { get { return Math.Round((double)winCount / enterCount, 4) * 100; } }
        public double ClaimTime { get { return claimTime; } }
        public double ClaimTimeAvg { get { return claimTimeAvg; } }

        public RaffleWin(JToken raffleWinProperties)
        {
            raffleJsonData = raffleWinProperties.ToString();
            winner = raffleWinProperties.SelectToken("winner").ToString();
            winCount = int.Parse(raffleWinProperties.SelectToken("win_count").ToString());
            enterCount = int.Parse(raffleWinProperties.SelectToken("enter_count").ToString());
            claimTime = double.Parse(raffleWinProperties.SelectToken("claim_time").ToString());
            claimTimeAvg = double.Parse(raffleWinProperties.SelectToken("claim_time_avg").ToString());
        }

        public override string ToString()
        {
            return raffleJsonData;
        }
    }
}
