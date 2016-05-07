using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KrakenBot2
{
    public class UptimeTracker
    {
        private Timer uptimeTimer = new Timer();
        private int uptimeMinutes = 0;

        public UptimeTracker()
        {
            uptimeTimer.Interval = 60000;
            uptimeTimer.Elapsed += uptimeTimerTick;
            uptimeTimer.Start();
        }

        private void uptimeTimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            uptimeMinutes += 1;
            setConsoleTitle();
        }

        private void setConsoleTitle()
        {
            int minutes = uptimeMinutes;
            int hours, days, weeks, months;
            string title = "";

            if(minutes >= 43800)
            {
                months = (minutes - (minutes % 43800)) / 43800;
                minutes -= 43800 * months;
                title = months + " month(s), ";
            }
            if(minutes >= 10080)
            {
                weeks = (minutes - (minutes % 10080)) / 10080;
                minutes -= 10080 * weeks;
                title += weeks + " week(s), ";
            }
            if(minutes >= 1440)
            {
                days = (minutes - (minutes % 1440)) / 1440;
                minutes -= 1440 * days;
                title += days + " day(s), ";
            }
            if(minutes >= 60)
            {
                hours = (minutes - (minutes % 60)) / 60;
                minutes -= 60 * hours;
                title += hours + " hour(s)";
            }
            if (minutes > 0)
                title += minutes + " minute(s)";

            Console.Title = "KrakenBot2 - Build: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + " - Uptime: " + title;
        }
    }
}
