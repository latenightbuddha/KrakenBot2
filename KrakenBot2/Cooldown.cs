using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2
{
    public class Cooldown
    {
        private List<CommandCooldown> cooldowns = new List<CommandCooldown>();
        private List<CommandCooldown> discordCooldowns = new List<CommandCooldown>();
        public Cooldown()
        {

        }

        public bool discordCommandAvailable(string command, int seconds)
        {
            if(seconds != 0)
            {
                foreach(CommandCooldown cooldown in discordCooldowns)
                {
                    if (cooldown.Command.ToLower() == command.ToLower())
                        return cooldown.activate();
                }
                discordCooldowns.Add(new CommandCooldown(command, seconds));
            }
            return true;
        }

        public bool commandAvailable(TwitchLib.ChatMessage.uType userType, string command, int seconds)
        {
            if(seconds != 0 && userType != TwitchLib.ChatMessage.uType.Moderator)
            {
                foreach (CommandCooldown cooldown in cooldowns)
                {
                    if (cooldown.Command.ToLower() == command.ToLower()) {
                        return cooldown.activate();
                    }
                }
                cooldowns.Add(new CommandCooldown(command, seconds));
            }
            return true;
        }

        class CommandCooldown
        {
            private string command;
            private int seconds;
            private double startTime;

            public string Command { get { return command; } }

            public CommandCooldown(string command, int seconds)
            {
                this.command = command;
                this.seconds = seconds;
                startTime = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
            }

            public bool activate()
            {
                if((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds - startTime > seconds)
                {
                    startTime = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
                    return true;
                } else
                {
                    return false;
                }
            }
        }
    }
}
