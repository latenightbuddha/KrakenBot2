using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.Objects
{
    public class Emote
    {
        private string emote, channel;
        private bool channelSpecific;

        public string EmoteString { get { return emote; } }
        public string Channel { get { return channel; } }
        public bool ChannelSpecific { get { return channelSpecific; } }

        public Emote(string emote, string channel, bool channelSpecific)
        {
            this.emote = emote;
            this.channel = channel;
            this.channelSpecific = channelSpecific;
        }
    }
}
