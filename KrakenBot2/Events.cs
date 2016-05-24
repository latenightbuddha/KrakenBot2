using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2
{
    public static class Events
    {
        public static bool connected = false;

        // Fires when channel state message is received from Twitch
        public static void onChannelState(object sender, TwitchLib.TwitchChatClient.OnChannelStateChangedArgs e)
        {
            if(!connected && !Common.DefaultOverride)
                Common.ChatClient.SendMessage(string.Format("/me [V2] connected[v{0}]!", Assembly.GetExecutingAssembly().GetName().Version), Common.DryRun);
            connected = true;
            if (!Common.EntryMessage)
                return;
        }

        // Fires when twitchnotify sends hosted streamer went offline
        public static void onHostedStreamerWentOffline(object sender, EventArgs e)
        {
            if (Common.Multihost != null)
                Common.Multihost.handleHostOfflineDetected();
        }

        // Fires when the chat client connects and changes to channel
        public static void chatOnConnect(object sender, TwitchLib.TwitchChatClient.OnConnectedArgs e)
        {
            Common.success("[CHAT]Connected to channel!");
        }

        // Turn on raw IRC messages here
        public static bool showRawIRC = true;

        // Fires when a chat message is received from chat client
        public static void chatOnMessage(object sender, TwitchLib.TwitchChatClient.OnMessageReceivedArgs e)
        {
            if(e.ChatMessage.Message[0] != '!')
            {
                if(showRawIRC)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Common.message(string.Format("MESSAGE {0}", e.ChatMessage.RawIrcMessage));
                } else
                {
                    Common.message(string.Format("MESSAGE {0}: {1}", e.ChatMessage.DisplayName, e.ChatMessage.Message));
                }
            }
            if (Common.Raffle != null && Common.Raffle.raffleIsActive())
                Common.Raffle.addEntry(e.ChatMessage.Username, e.ChatMessage.Message);
            ChatFiltering.violatesProtections(e.ChatMessage.Username, Common.isSub(e), Common.isMod(e), e.ChatMessage.Message);
            if (Common.AhoyRewarder.isActive())
                Common.AhoyRewarder.processMessage(e);
            processPotentialSub(e);
            Common.ChatMessageTracker.addMessage(e.ChatMessage);
        }

        // Fires when a message is received that is prefixed with an !
        public static void chatOnCommand(object sender, TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            ChatFiltering.violatesProtections(e.ChatMessage.Username, Common.isSub(e), Common.isMod(e), e.ChatMessage.Message);
            Commands.handleChatCommand(e);
            processPotentialSub(e);
            Common.ChatMessageTracker.addMessage(e.ChatMessage);
        }

        // Fires when a subscription from channel connected to occurs (noteably not a hosted streamer's sub)
        public static void chatOnSubscribe(object sender, TwitchLib.TwitchChatClient.OnSubscriberArgs e)
        {
            Common.other(string.Format("SUBSCRIBER {0}, months: ", e.Subscriber.Months));
            Subscriptions.handleSubscription(e);
        }

        // Fires when whisper client connects to twitch group chat server
        public static void whisperOnConnected(object sender, TwitchLib.TwitchWhisperClient.OnConnectedArgs e)
        {
            Common.success(string.Format("[WHISPER]Connected to group chat!"));
        }

        // Fires when a whisper is received from whisper client
        public static void whisperOnWhisper(object sender, TwitchLib.TwitchWhisperClient.OnWhisperReceivedArgs e)
        {
            if(e.WhisperMessage.Message[0] != '!')
                Common.other(string.Format("WHISPER {0}: {1}", e.WhisperMessage.DisplayName, e.WhisperMessage.Message));
        }

        // Fires when a whisper is received that is prefixed with an !
        public static void whisperOnCommand(object sender, TwitchLib.TwitchWhisperClient.OnCommandReceivedArgs e)
        {
            Common.command(string.Format("[WHISPER]COMMAND {0}: {1}", e.Username, e.Command + e.ArgumentsAsString), true);
            Commands.handleWhisperCommand(e);
        }

        // Fires when a message is received from the raid instance chat client
        public static void raidOnMessage(object sender, TwitchLib.TwitchChatClient.OnMessageReceivedArgs e)
        {
            Common.other(string.Format("[RAID]MESSAGE {0}: {1}", e.ChatMessage.DisplayName, e.ChatMessage.Message));
            if (Common.RaidInstance != null)
                Common.RaidInstance.handleMessage(e);
        }

        // Fires when bot connects to discord server
        public static void onDiscordConnected(Object s, Discord.ServerEventArgs e)
        {
            Common.DiscordClient.SetGame("BurkeBlack.TV");
            Common.relay("connected!");
        }

        // Fires when bot receives a message from any discord channel
        public static void onDiscordMessageReceived(object s, Discord.MessageEventArgs e)
        {
            
            if (e.Message.Text.Length > 0 && e.Message.Text[0] == '!')
                Commands.handleDiscordCommand(new Objects.DiscordCommand(e.User.Name, e.Message.Text, e.Channel.Name, e.Channel.Id));
            if(e.Channel.Name.ToLower() == "relay")
            {
                if (e.Message.Text.ToLower() == "!restart")
                {
                    Common.relay("Restarting... Please standby!");
                    System.Diagnostics.Process.Start(Assembly.GetExecutingAssembly().Location);
                    Environment.Exit(0);
                }
            }
            Console.WriteLine(string.Format("[{0}] {1}: {2}", e.Channel.Name, e.User.Name, e.Message.Text));
        }

        // Internal function to process a received sub and redirect stack to ChatSubs object utilizing chat message as identity
        private static void processPotentialSub(TwitchLib.TwitchChatClient.OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Subscriber && !Common.ChatSubs.Contains(e.ChatMessage.Username.ToLower()))
                Common.ChatSubs.Add(e.ChatMessage.Username.ToLower());
        }

        // Internal function to process a received sub and redirect stack to ChatSubs object utilizing command as identity
        private static void processPotentialSub(TwitchLib.TwitchChatClient.OnCommandReceivedArgs e)
        {
            if (e.ChatMessage.Subscriber && !Common.ChatSubs.Contains(e.ChatMessage.Username.ToLower()))
                Common.ChatSubs.Add(e.ChatMessage.Username.ToLower());
        }
    }
}
