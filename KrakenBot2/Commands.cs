using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2
{
    public static class Commands
    {
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
                    Common.WhisperClient.sendWhisper(command.Username, "To view available whisper commands, whisper the bot !commands", Common.DryRun);
                    //Handle dynamically created whisper commands
                    break;
            }
        }
        public static void handleChatCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs command)
        {
            switch(command.Command)
            {
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
                case "checkhost":
                    HardCodedChatCommands.Multihost.CheckHost.handleCommand(command);
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
                case "game":
                    HardCodedChatCommands.Game.handleCommand(command);
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
                        foreach(Objects.ChatCommand dynCommand in Common.ChatCommands)
                        {
                            if(dynCommand.Command == command.Command)
                            {
                                if (validateTiers(command, dynCommand))
                                    if(Common.Cooldown.commandAvailable(command.ChatMessage.UserType, command.Command, dynCommand.Cooldown))
                                    {
                                        string msg = processDynamicVariables(command, dynCommand);
                                        if(msg != "")
                                            Common.ChatClient.sendMessage(msg, Common.DryRun);
                                    }
                            }
                        }
                    }
                    break;
            }
        }

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

        private static string processDynamicVariables(TwitchLib.TwitchChatClient.CommandReceivedArgs e, Objects.ChatCommand command)
        {
            if (e.ArgumentsAsList.Count >= (command.ArgsAsList.Count + 1))
                return "";
            string retStr = command.Return;      
            retStr = retStr.Replace("{sender}", Variables.SENDER(e));
            retStr = retStr.Replace("{recent_sub_name}", Variables.RECENT_SUB_NAME());
            retStr = retStr.Replace("{recent_sub_length}", Variables.RECENT_SUB_LENGTH());
            retStr = retStr.Replace("{stream_game}", Variables.STREAM_GAME());
            retStr = retStr.Replace("{stream_link}", Variables.STREAM_LINK());
            retStr = retStr.Replace("{stream_status}", Variables.STREAM_STATUS());
            retStr = retStr.Replace("{viewer_count}", Variables.VIEWER_COUNT());
            retStr = retStr.Replace("{command_count}", Variables.COMMAND_COUNT());
            retStr = retStr.Replace("{rotating_message_count}", Variables.ROTATING_MESSAGE_COUNT());
            retStr = retStr.Replace("{online_status}", Variables.ONLINE_STATUS());
            retStr = retStr.Replace("{raffle_name}", Variables.RAFFLE_NAME());
            retStr = retStr.Replace("{raffle_donator}", Variables.RAFFLE_DONATOR());
            retStr = retStr.Replace("{raffle_author}", Variables.RAFFLE_AUTHOR());

            if (e.ArgumentsAsList.Count > 0)
            {
                int currentReplace = 0;
                foreach (string arg in e.ArgumentsAsList)
                {
                    retStr = retStr.Replace(command.ArgsAsList[currentReplace], e.ArgumentsAsList[currentReplace]);
                    currentReplace++;
                }
            }

            return retStr;
        }
    }
}
