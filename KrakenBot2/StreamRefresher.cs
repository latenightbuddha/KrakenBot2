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
        private TwitchLib.TwitchAPIClasses.TwitchStream stream;
        private Timer refresher = new Timer(60000);

        public TwitchLib.TwitchAPIClasses.TwitchStream Stream { get { return stream; } }

        public StreamRefresher()
        {
            stream = TwitchLib.TwitchAPI.getTwitchStream("burkeblack");
            refresher.Elapsed += refresherTick;
            refresher.Start();
        }

        public bool isOnline()
        {
            return stream != null;
        }

        public bool isOffline()
        {
            return stream == null;
        }

        private void refresherTick(object sender, ElapsedEventArgs e)
        {
            stream = TwitchLib.TwitchAPI.getTwitchStream("burkeblack");
        }
    }
}
