using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Timers;

namespace KrakenBot2
{
    public class MessageRotator
    {
        private List<RotatingMessage> messages = new List<RotatingMessage>();
        private List<RotatingMessage> dedicatedMessages = new List<RotatingMessage>();
        private Timer messageTimer = new Timer(1000);
        private int currentMessageIndex = 0;
        private int nextMessageIn;

        public List<RotatingMessage> Messages { get { return messages; } }
        public List<RotatingMessage> DedicatedMessages { get { return dedicatedMessages; } }
        public RotatingMessage Current_Message { get { return messages[currentMessageIndex]; } }
        public int Seconds_Remaining { get { return nextMessageIn; } }

        // Message rotator constructor using API data received from burkeblack.tv
        public MessageRotator(string data)
        {
            foreach(JToken message in JObject.Parse(data).SelectToken("messages"))
            {
                RotatingMessage newMsg = new RotatingMessage(message);
                if (newMsg.Dedicated)
                    dedicatedMessages.Add(newMsg);
                else
                    messages.Add(newMsg);
            }
            nextMessageIn = messages[0].Interval;
            messageTimer.Elapsed += onTick;
            messageTimer.Start();
            
        }

        // Starts message timer
        public void Start()
        {
            messageTimer.Start();
        }

        // Stops message timer
        public void Stop()
        {
            messageTimer.Stop();
        }

        // MessageRotator timer tick event
        public void onTick(object sender, ElapsedEventArgs e)
        {
            if (nextMessageIn == 0)
            {
                if (currentMessageIndex == messages.Count - 1)
                    currentMessageIndex = 0;
                else
                    currentMessageIndex += 1;
                nextMessageIn = messages[currentMessageIndex].Interval;
                
                Common.ChatClient.SendMessage(string.Format("{0}", messageReplacements(messages[currentMessageIndex].Contents)), Common.DryRun);
            } else
            {
                nextMessageIn--;
            }
            foreach(RotatingMessage rotMsg in dedicatedMessages)
            {
                if(rotMsg.dedicatedValid())
                {
                    Common.ChatClient.SendMessage(string.Format("{0}", messageReplacements(rotMsg.Contents)));
                }
            }
        }

        // Method to handle API data and use it to refresh local rotating message cache
        public void refreshMessages(string data)
        {
            messages.Clear();
            foreach (JToken message in JObject.Parse(data).SelectToken("messages"))
            {
                messages.Add(new RotatingMessage(message));
            }
            nextMessageIn = messages[0].Interval;
        }

        // Function to replace various message place holders with data retrieved internally or externally
        private string messageReplacements(string message)
        {
            if(message.Contains("[recent_twitch]"))
            {
                TwitchLib.TwitchAPIClasses.TwitchVideo recentVideo = TwitchLib.TwitchApi.GetChannelVideos("burkeblack", 1).Result[0];
                if (recentVideo == null)
                    return message.Replace("[recent_twitch]", "Not Available - query failed");
                TimeSpan ts = new TimeSpan(0, 0, recentVideo.Length);
                string duration = string.Format("{0}:{1}", ts.Minutes, ts.Seconds);
                message = message.Replace("[recent_twitch]", string.Format("{0} [Views: {1}] [Duration: {2}] - {3}", recentVideo.Title, recentVideo.Views, duration, recentVideo.Url));
            }
            if(message.Contains("[recent_youtube]"))
            {
                Objects.YoutubeVideo recentVideo = WebCalls.getBurkesLatestYTVideo().Result;
                if (recentVideo == null)
                    return message.Replace("[recent_youtube]", "Not Available - query failed");
                message = message.Replace("[recent_youtube]", string.Format("{0} [Views: {1}] - http://youtu.be/{2}", recentVideo.Title, recentVideo.Views, recentVideo.VideoID));
            }
            return message;
        }
    }

    // Class representing the properties of a rotating message
    public class RotatingMessage
    {
        private string name, contents;
        private bool dedicated = false;
        private int interval;
        private int curSeconds = 0;

        public string Name { get { return name; } }
        public string Contents { get { return contents; } }
        public int Interval { get { return interval; } }
        public bool Dedicated { get { return dedicated; } }

        // RotatingMessage constructor using JSON data from API
        public RotatingMessage(JToken data)
        {
            name = data.SelectToken("name").ToString();
            contents = data.SelectToken("contents").ToString();
            interval = int.Parse(data.SelectToken("interval").ToString());
            if (data.SelectToken("dedicated_timer").ToString() == "1")
                dedicated = true;
        }

        // Increments if message is deemed dedicated
        public bool dedicatedValid()
        {
            curSeconds++;
            if (curSeconds >= interval)
            {
                curSeconds = 0;
                return true;
            }
            return false;
        }
    }
}
