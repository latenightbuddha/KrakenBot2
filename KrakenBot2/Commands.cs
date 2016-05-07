using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2
{
    public static class Commands
    {
        // Handles all discord commands (messages that have ! prefix) from event
        public static void handleDiscordCommand(Objects.DiscordCommand command)
        {
            switch(command.Command)
            {
                case "uptime":
                    DiscordPublicCommands.Uptime.handleCommand(command);
                    break;
                case "doubloons":
                    DiscordPublicCommands.Doubloons.handleCommand(command);
                    break;
                default:
                    break;
            }
        }

        // Handles all whisper commands (whispers that have ! prefix) from event
        public static void handleWhisperCommand(TwitchLib.TwitchWhisperClient.CommandReceivedArgs command)
        {
            switch(command.Command)
            {
                case "notifyme":
                    HardCodedWhisperCommands.OnlineNotifications.NotifyMe.handleCommand(command);
                    break;
                case "removeme":
                    HardCodedWhisperCommands.OnlineNotifications.RemoveMe.handleCommand(command);
                    break;
                case "doubloons":
                    HardCodedWhisperCommands.Doubloons.handleCommand(command);
                    break;
                case "commands":
                    HardCodedWhisperCommands.Commands.handleCommand(command);
                    break;
                case "discordinvite":
                    HardCodedWhisperCommands.DiscordInvite.handleCommand(command);
                    break;
                case "giveawayhistory":
                    HardCodedWhisperCommands.GiveawayHistory.handleCommand(command);
                    break;
                default:
                    if (Common.WhisperClient != null)
                        Common.WhisperClient.sendWhisper(command.Username, "To view available whisper commands, whisper the bot !commands", Common.DryRun);
                    //Handle dynamically created whisper commands
                    break;
            }
        }

        // Handles all chat commands (chat messages that have ! prefix) from event
        public static void handleChatCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs command)
        {
            switch(command.Command)
            {
                case "drink":
                    // Intentionally fall through
                case "drinking":
                    HardCodedChatCommands.Drink.handleCommand(command);
                    break;
                case "500":
                    HardCodedChatCommands._500.handleCommand(command);
                    break;
                case "killgiveaway":
                    HardCodedChatCommands.Raffle.KillGiveaway.handleCommand(command);
                    break;
                case "giveawaykill":
                    HardCodedChatCommands.Raffle.KillGiveaway.handleCommand(command);
                    break;
                case "games":
                    HardCodedChatCommands.Games.handleCommand(command);
                    break;
                case "pick":
                    HardCodedChatCommands.Pick.handleCommand(command);
                    break;
                case "kappa":
                    HardCodedChatCommands.Kappa.handleCommand(command);
                    break;
                case "dc":
                    HardCodedChatCommands.DeathCount.handleCommand(command);
                    break;
                case "deathcount":
                    HardCodedChatCommands.DeathCount.handleCommand(command);
                    break;
                case "hc":
                    HardCodedChatCommands.HitCount.handleCommand(command);
                    break;
                case "hitcount":
                    HardCodedChatCommands.HitCount.handleCommand(command);
                    break;
                case "notifyme":
                    HardCodedChatCommands.OnlineNotifications.NotifyMe.handleCommand(command);
                    break;
                case "removeme":
                    HardCodedChatCommands.OnlineNotifications.RemoveMe.handleCommand(command);
                    break;
                case "kraken":
                    HardCodedChatCommands.Kraken.handleCommand(command);
                    break;
                case "restart":
                    HardCodedChatCommands.Restart.handleCommand(command);
                    break;
                case "recentsub":
                    HardCodedChatCommands.RecentSub.handleCommand(command);
                    break;
                case "recentdonation":
                    HardCodedChatCommands.RecentDonation.handleCommand(command);
                    break;
                case "permit":
                    HardCodedChatCommands.Permit.handleCommand(command);
                    break;
                case "claim":
                    HardCodedChatCommands.Raffle.Claim.handleCommand(command);
                    break;
                case "enter":
                    HardCodedChatCommands.Raffle.Enter.handleCommand(command);
                    break;
                case "pass":
                    HardCodedChatCommands.Raffle.Pass.handleCommand(command);
                    break;
                case "extend":
                    HardCodedChatCommands.Multihost.Extend.handleCommand(command);
                    break;
                case "remaining":
                    HardCodedChatCommands.Multihost.Remaining.handleCommand(command);
                    break;
                case "next":
                    HardCodedChatCommands.Multihost.Guess.handleCommand(command);
                    break;
                case "follow":
                    HardCodedChatCommands.Follow.handleCommand(command);
                    break;
                case "highlight":
                    HardCodedChatCommands.Highlight.handleCommand(command);
                    break;
                case "mods":
                    HardCodedChatCommands.Mods.handleCommand(command);
                    break;
                case "quote":
                    HardCodedChatCommands.Quote.handleCommand(command);
                    break;
                case "raid":
                    HardCodedChatCommands.Raid.handleCommand(command);
                    break;
                case "raise":
                    HardCodedChatCommands.Raise.handleCommand(command);
                    break;
                case "talk":
                    HardCodedChatCommands.Talk.handleCommand(command);
                    break;
                case "time":
                    HardCodedChatCommands.Time.handleCommand(command);
                    break;
                case "uptime":
                    HardCodedChatCommands.Uptime.handleCommand(command);
                    break;
                case "w":
                    HardCodedChatCommands.Weather.handleCommand(command);
                    break;
                case "title":
                    HardCodedChatCommands.Title.handleCommand(command);
                    break;
                case "changegame":
                    HardCodedChatCommands.changegame.handleCommand(command);
                    break;
                case "gt":
                    HardCodedChatCommands.Gamertag.handleCommand(command);
                    break;
                default:
                    if(command.Command.ToLower() == command.ChatMessage.Username.ToLower())
                    {
                        //Handle personal command
                        HardCodedChatCommands.Personal.handleCommand(command);
                    } else
                    {
                        //Handle dynamically created chat commands
                        TwitchLib.ChatMessage.uType userType = command.ChatMessage.UserType;
                        if (Common.Moderators.Contains(command.ChatMessage.Username.ToLower()))
                            userType = TwitchLib.ChatMessage.uType.Moderator;
                        bool found = false;
                        foreach(Objects.ChatCommand dynCommand in Common.ChatCommands)
                        {
                            if(dynCommand.Command.ToLower() == command.Command.ToLower())
                            {
                                if (validateTiers(command, dynCommand))
                                {
                                    if (Common.Cooldown.chatCommandAvailable(userType, command.Command, dynCommand.Cooldown))
                                    {
                                        Console.WriteLine(dynCommand.ReturnMessages[0]);
                                        List<string> msgs = processDynamicVariables(command, dynCommand);
                                        Console.WriteLine(dynCommand.ReturnMessages[0]);
                                        foreach(string msg in msgs)
                                            if(msg != "")
                                                Common.ChatClient.sendMessage(msg, Common.DryRun);
                                    }
                                }
                                found = true;
                            }
                        }
                        if(!found)
                        {
                            foreach(Objects.ChatCommand dynCommand in Common.ChatCommands)
                            {
                                Console.WriteLine(dynCommand.Command);
                                foreach(string returnMessage in dynCommand.ReturnMessages)
                                {
                                    if (returnMessage.ToLower().Contains(" " + command.Command.ToLower() + " ") && validateTiers(command, dynCommand))
                                    {
                                        if (Common.Cooldown.chatCommandAvailable(userType, command.Command, dynCommand.Cooldown))
                                        {
                                            List<string> msgs = processDynamicVariables(command, dynCommand);
                                            foreach(string msg in msgs)
                                                if(msg != "")
                                                {
                                                    Common.ChatClient.sendMessage(string.Format("Did you mean !{0}? {1}", dynCommand.Command, msg), Common.DryRun);
                                                    return;
                                                }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }

        // Determine if command received's UserTier meets command UserTier in descending fashion
        private static bool validateTiers(TwitchLib.TwitchChatClient.CommandReceivedArgs e, Objects.ChatCommand command)
        {
            switch(command.UserTier)
            {
                case Objects.ChatCommand.uTier.SWIFTYSPIFFY:
                    if (e.ChatMessage.Username.ToLower() == "swiftyspiffy")
                        return true;
                    return false;
                case Objects.ChatCommand.uTier.MODERATOR:
                    if (Common.isMod(e))
                        return true;
                    return false;
                case Objects.ChatCommand.uTier.SUBSCRIBER:
                    if (Common.isSub(e))
                        return true;
                    return false;
                case Objects.ChatCommand.uTier.VIEWER:
                    return true;
                case Objects.ChatCommand.uTier.DISABLED:
                    return false;
                default:
                    return true;
            }
        }

        // Replaces dynamic variables in command or command return strings
        private static List<string> processDynamicVariables(TwitchLib.TwitchChatClient.CommandReceivedArgs e, Objects.ChatCommand command)
        {
            if (e.ArgumentsAsList.Count >= (command.ArgsAsList.Count + 1))
                return new List<string>();
            List<string> returnMessages = command.ReturnMessages.ToList();  
            
            for(int i = 0; i < returnMessages.Count; i++)
            {
                returnMessages[i] = returnMessages[i].Replace("{sender}", Variables.SENDER(e));
                returnMessages[i] = returnMessages[i].Replace("{recent_sub_name}", Variables.RECENT_SUB_NAME());
                returnMessages[i] = returnMessages[i].Replace("{recent_sub_length}", Variables.RECENT_SUB_LENGTH());
                returnMessages[i] = returnMessages[i].Replace("{stream_game}", Variables.STREAM_GAME());
                returnMessages[i] = returnMessages[i].Replace("{stream_link}", Variables.STREAM_LINK());
                returnMessages[i] = returnMessages[i].Replace("{stream_status}", Variables.STREAM_STATUS());
                returnMessages[i] = returnMessages[i].Replace("{viewer_count}", Variables.VIEWER_COUNT());
                returnMessages[i] = returnMessages[i].Replace("{command_count}", Variables.COMMAND_COUNT());
                returnMessages[i] = returnMessages[i].Replace("{rotating_message_count}", Variables.ROTATING_MESSAGE_COUNT());
                returnMessages[i] = returnMessages[i].Replace("{online_status}", Variables.ONLINE_STATUS());
                returnMessages[i] = returnMessages[i].Replace("{raffle_name}", Variables.RAFFLE_NAME());
                returnMessages[i] = returnMessages[i].Replace("{raffle_donator}", Variables.RAFFLE_DONATOR());
                returnMessages[i] = returnMessages[i].Replace("{raffle_author}", Variables.RAFFLE_AUTHOR());

                if (e.ArgumentsAsList.Count > 0)
                {
                    int currentReplace = 0;
                    foreach (string arg in e.ArgumentsAsList)
                    {
                        returnMessages[i] = returnMessages[i].Replace(command.ArgsAsList[currentReplace], e.ArgumentsAsList[currentReplace]);
                        currentReplace++;
                    }
                }
            }

            return returnMessages;
        }
    }
}
