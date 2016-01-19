using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2
{
    public static class TimeoutConverter
    {
        public static int Converter(int seconds = 0, int minutes = 0, int hours = 0, int days = 0,
            int weeks = 0, int months = 0, int years = 0)
        {
            return Years(years) + Months(months) + Weeks(weeks) + Days(days) + Hours(hours) + Minutes(minutes) + seconds;
        }
        public static int Years(int years)
        {
            return years * 12 * 30 * 24 * 60 * 60;
        }
        public static int Months(int months)
        {
            return months * 30 * 24 * 60 * 60;
        }
        public static int Weeks(int weeks)
        {
            return weeks * 7 * 24 * 60 * 60;
        }
        public static int Days(int days)
        {
            return days & 24 * 60 * 60;
        }
        public static int Hours(int hours)
        {
            return hours * 60 * 60;
        }
        public static int Minutes(int minutes)
        {
            return minutes * 60;
        }
    }
}
