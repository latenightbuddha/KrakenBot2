using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KrakenBot2
{
    public class ChatMessageTracker
    {
        // Send query to update counts every 30 minutes
        private Timer messageCountUploader = new Timer(1800000);
        private List<UserMessages> userMessages = new List<UserMessages>();

        // ChatMessageTracker constructor
        public ChatMessageTracker()
        {
            messageCountUploader.Elapsed += messageCountUploaderTick;
            messageCountUploader.Start();
        }

        // Add message to userMessages list
        public void addMessage(TwitchLib.ChatMessage e)
        {
            bool found = false;
            foreach(UserMessages userMessage in userMessages)
            {
                if (userMessage.Username == e.Username)
                {
                    userMessage.incrementMessages();
                    found = true;
                }
            }
            if (!found)
                userMessages.Add(new UserMessages(e.Username, 1));
        }

        // messageCountUploader timer tick event
        private void messageCountUploaderTick(object sender, ElapsedEventArgs e)
        {
            if(userMessages.Count > 0)
            {
                string uploadStr = "";
                foreach (UserMessages userMessage in userMessages)
                {
                    if (uploadStr == "")
                        uploadStr = string.Format("{0},{1}", userMessage.Username, userMessage.Messages);
                    else
                        uploadStr = string.Format("{0}|{1},{2}", uploadStr, userMessage.Username, userMessage.Messages);
                }
                WebCalls.uploadChatMessageCounts(uploadStr);
                userMessages.Clear();
            }
        }

        // UserMessages class, contains username and message count
        private class UserMessages
        {
            private string username;
            private int messages;

            public string Username { get { return username; } }
            public int Messages { get { return messages; } }

            // UserMessages constructor
            public UserMessages(string username, int messages)
            {
                this.username = username;
                this.messages = messages;
            }

            // Increment the current message count for user
            public int incrementMessages()
            {
                return ++messages;
            }
        }
    }
}
