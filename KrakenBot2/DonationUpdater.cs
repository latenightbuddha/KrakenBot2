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
        private bool enabled = true;

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
                //Check to see if query failed
                if (recentDonations == null)
                    return;
                if (recentDonations.Count > 0)
                {
                    foreach (Objects.RecentDonation donation in recentDonations)
                    {
                        Common.ChatClient.sendMessage(string.Format("NEW DONATION! {0} donated ${1} with the message '{2}'. Thanks for supporting the Black Crew!",
                            donation.Username, donation.Amount, donation.Message));
                    }
                    foreach (Objects.RecentDonation donation in Common.RecentDonations)
                        recentDonations.Add(donation);
                    Common.RecentDonations = recentDonations;
                }
            }
        }
    }
}
