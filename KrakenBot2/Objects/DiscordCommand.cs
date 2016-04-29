using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.Objects
{
    public class DiscordCommand
    {
        private string user;
        private string channel;
        private string command;
        private string commandsAsString;
        private List<string> commandsAsList = new List<string>();
        private ulong id;
        private bool private_msg;

        public string User { get { return user; } }
        public string Channel { get { return channel; } }
        public string Command { get { return command; } }
        public string CommandsAsString { get { return commandsAsString; } }
        public List<string> CommandsAsList { get { return commandsAsList; } }
        public ulong Channel_ID { get { return id; } }
        public bool PrivateMessage { get { return private_msg; } }

        public DiscordCommand(string user, string message, string channel_from, ulong id, bool priv_msg = false)
        {
            this.id = id;
            this.user = user;
            channel = channel_from;
            private_msg = priv_msg;
            if(message.Contains(" "))
            {
                commandsAsString = message.Replace(message.Split(' ') + " ", "");
                int i = 0;
                foreach(string word in message.Split(' '))
                {
                    if(i != 0)
                    {
                        commandsAsList.Add(word);
                    }
                    i++;
                }
                command = message.Split(' ')[0].Split('!')[1];
            } else
            {
                command = message.Split('!')[1];
            }
        }
    }
}
