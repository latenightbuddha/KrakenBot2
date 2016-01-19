using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KrakenBot2
{
    public class DonationUpdater
    {
        private bool enabled = false;

        private Timer donationTimer = new Timer(60000);
		public DonationUpdater()
        {
            donationTimer.Elapsed += donationTimerTick;
            if(enabled)
                donationTimer.Start();
        }

		private void donationTimerTick(object sender, ElapsedEventArgs e)
        {
			if(Common.StreamRefresher.isOnline())
            {
                List<Objects.RecentDonation> recentDonations = WebCalls.downloadRecentDonations().Result;
                if (recentDonations.Count > 0)
                {
                    foreach (Objects.RecentDonation donation in recentDonations)
                    {
                        Common.ChatClient.sendMessage(string.Format("NEW DONATION! {0} donated ${1} with the message '{2}' approx. {3} seconds ago! Thanks for supporting the Black Crew!",
                            donation.Username, donation.Amount, donation.Message, DateTime.Now.Subtract(donation.Date).Seconds));
                    }
                    foreach (Objects.RecentDonation donation in Common.RecentDonations)
                        recentDonations.Add(donation);
                    Common.RecentDonations = recentDonations;
                }
            }
        }
    }
}
