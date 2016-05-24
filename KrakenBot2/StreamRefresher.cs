using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KrakenBot2
{

    public class StreamRefresher
    {
        // Configurable variables
        private Timer refresher = new Timer(60000);

        private TwitchLib.TwitchAPIClasses.TwitchStream stream;

        public TwitchLib.TwitchAPIClasses.TwitchStream Stream { get { return stream; } }

        // StreamRefresh constructor
        public StreamRefresher()
        {
            stream = TwitchLib.TwitchApi.GetTwitchStream("burkeblack").Result;
            refresher.Elapsed += refresherTick;
            refresher.Start();
        }

        // Returns true if burkeblack is online, false if he is offline
        public bool isOnline()
        {
            return stream != null;
        }

        // StreamRefresher timer tick event
        private void refresherTick(object sender, ElapsedEventArgs e)
        {
            stream = TwitchLib.TwitchApi.GetTwitchStream("burkeblack").Result;
        }
    }
}
