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
        
        // Cooldown object constructor
        public Cooldown() { }

        // Checks availability of discord command, and resets cooldown if available
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

        // Checks availability of chat command, and resets cooldown if available
        public bool chatCommandAvailable(TwitchLib.ChatMessage.UType userType, string command, int seconds)
        {
            if(seconds != 0 && userType != TwitchLib.ChatMessage.UType.Moderator)
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

        // Class that represents a command's cooldown variables
        private class CommandCooldown
        {
            private string command;
            private int seconds;
            private double startTime;

            public string Command { get { return command; } }

            // CommandCooldown constructor accepting both command, and cooldown seconds
            public CommandCooldown(string command, int seconds)
            {
                this.command = command;
                this.seconds = seconds;
                startTime = (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
            }

            // Determines if command is available, and resets time if it is
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
