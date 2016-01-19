using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KrakenBot2.Objects
{
    public class RecentDonation
    {
        private string username, message;
        private DateTime date;
        private double amount;

        public string Username { get { return username; } }
        public string Message { get { return message; } }
        public DateTime Date { get { return date; } }
        public Double Amount { get { return amount; } }

        public RecentDonation(JToken donationData)
        {
            username = donationData.SelectToken("username").ToString();
            message = donationData.SelectToken("message").ToString();
            date = Convert.ToDateTime(donationData.SelectToken("date").ToString());
            amount = Double.Parse(donationData.SelectToken("amount").ToString());
        }
    }
}
