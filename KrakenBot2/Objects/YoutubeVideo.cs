using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KrakenBot2.Objects
{
    public class YoutubeVideo
    {
        private string title, description, videoID;
        private int views, likes, dislikes, favs, comments;

        public string Title { get { return title; } }
        public string Description { get { return description; } }
        public string VideoID { get { return videoID; } }
        public int Views { get { return views; } }
        public int Dislikes { get { return dislikes; } }
        public int Favs { get { return favs; } }
        public int Comments { get { return comments; } }

        public YoutubeVideo(string title, string description, string videoID, int views, int likes, int dislikes, int favs, int comments)
        {
            this.title = title;
            this.description = description;
            this.videoID = videoID;
            this.views = views;
            this.likes = likes;
            this.dislikes = dislikes;
            this.favs = favs;
            this.comments = comments;
        }
    }
}
