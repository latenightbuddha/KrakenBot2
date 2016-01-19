using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KrakenBot2.Objects
{
    public class YoutubeStats
    {
        private int viewCount, commentCount, subscriberCount, videoCount;
        
        public int ViewCount { get { return viewCount; } }
        public int CommentCount { get { return commentCount; } }
        public int SubscriberCount { get { return subscriberCount; } }
        public int VideoCount { get { return videoCount; } }

        public YoutubeStats(JToken youtubeData)
        {
            viewCount = int.Parse(youtubeData.SelectToken("viewCount").ToString());
            commentCount = int.Parse(youtubeData.SelectToken("commentCount").ToString());
            subscriberCount = int.Parse(youtubeData.SelectToken("subscriberCount").ToString());
            videoCount = int.Parse(youtubeData.SelectToken("videoCount").ToString());
        }
    }
}
