using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.Objects
{
    public class Permit
    {
        private string username;
        private DateTime original;
        private int totalUsages;
        private int currentUsages = 0;

        public string Username { get { return username; } }

        public Permit(string username, DateTime original, int totalUsages)
        {
            this.username = username;
            this.original = original;
            this.totalUsages = totalUsages;
        }

        public void update(DateTime original, int totalUsages)
        {
            this.original = original;
            this.totalUsages = totalUsages;
            currentUsages = 0;
        }

        public bool usePermit()
        {
            TimeSpan difference = DateTime.Now - original;
            if(difference.Minutes <= 3 && currentUsages < totalUsages)
            {
                currentUsages++;
                return true;
            } else
            {
                return false;
            }
        }
    }
}
