using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KrakenBot2.Objects
{
    public class ChatCommand
    {
        //Command tiers inherit in downward fashion
        public enum uTier
        {
            VIEWER,
            SUBSCRIBER,
            MODERATOR,
            SWIFTYSPIFFY,
            DISABLED
        }

        private uTier userTier;
        private string command;
        private List<string> returnMessages = new List<string>();
        private int secondCooldown;
        private List<string> argsAsList = new List<string>();
        private string argsAsString;

        public uTier UserTier { get { return userTier; } }
        public string Command { get { return command; } }
        public List<string> ReturnMessages { get { return returnMessages; } }
        public int Cooldown { get { return secondCooldown; } }
        public List<string> ArgsAsList { get { return argsAsList; } }
        public string ArgsAsString { get { return argsAsString; } }

        public ChatCommand(JToken data)
        {
            command = data.SelectToken("command").ToString();
            if (command[0] == '!')
                if (command.Contains(' '))
                {
                    foreach (string arg in command.Split(' '))
                        if (arg[0] != '!')
                            argsAsList.Add(arg);
                    command = command.Split(' ')[0].Substring(1, command.Split(' ')[0].Length - 1);
                } else
                {
                    command = command.Substring(1, command.Length - 1);
                }
            if (!data.SelectToken("return").ToString().Contains("|"))
            {
                returnMessages.Add(data.SelectToken("return").ToString());
            }
            else
            {
                foreach (string message in data.SelectToken("return").ToString().Split('|'))
                    returnMessages.Add(message);
            }
            secondCooldown = int.Parse(data.SelectToken("cooldown").ToString());
            switch(data.SelectToken("tier").ToString())
            {
                case "viewer":
                    userTier = uTier.VIEWER;
                    break;
                case "subscriber":
                    userTier = uTier.SUBSCRIBER;
                    break;
                case "moderator":
                    userTier = uTier.MODERATOR;
                    break;
                case "swiftyspiffy":
                    userTier = uTier.SWIFTYSPIFFY;
                    break;
                default:
                    userTier = uTier.DISABLED;
                    break;
            }

            foreach(string arg in argsAsList)
            {
                if (argsAsString == "")
                    argsAsString = arg;
                else
                    argsAsString = string.Format("{0} {1}", argsAsString, arg);
            }
        }
    }
}
