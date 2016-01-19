using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.Objects
{
    public class FollowData
    {
        private TwitchLib.TwitchChannel channel;
        private string chatMessage;

        public TwitchLib.TwitchChannel Channel { get { return channel; } }
        public string ChatMessage { get { return chatMessage; } }
        public FollowData(TwitchLib.TwitchChannel channel)
        {
            this.channel = channel;
            if (channel.Mature)
                chatMessage = string.Format("Go follow {0} (mature channel), they've been playing: {1}! http://twitch.tv/{0}", channel.Name, channel.Game);
            else
                chatMessage = string.Format("Go follow {0}, they've been playing: {1}! http://twitch.tv/{0}", channel.Name, channel.Game);
        }
    }
}
