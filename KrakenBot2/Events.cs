using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DiscordSharp;
using DiscordSharp.Events;

namespace KrakenBot2
{
    public static class Events
    {
        public static bool connected = false;
        public static void onChannelState(object sender, TwitchLib.TwitchChatClient.ChannelStateAssignedArgs e)
        {
            if(!connected)
                Common.ChatClient.sendMessage(string.Format("/me [V2] connected[v{0}]!", Assembly.GetExecutingAssembly().GetName().Version), Common.DryRun);
            connected = true;
            if (!Common.EntryMessage)
                return;
        }

        public static void onHostedStreamerWentOffline(object sender, EventArgs e)
        {
            if (Common.Multihost != null)
                Common.Multihost.handleHostOfflineDetected();
        }

        public static void chatOnConnect(object sender, TwitchLib.TwitchChatClient.OnConnectedArgs e)
        {
            Common.success("[CHAT]Connected to channel!");
        }

        public static bool showRawIRC = true;
        public static void chatOnMessage(object sender, TwitchLib.TwitchChatClient.NewChatMessageArgs e)
        {
            if(e.ChatMessage.Message[0] != '!')
            {
                if(showRawIRC)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Common.message(string.Format("MESSAGE {0}", e.ChatMessage.RawIRCMessage));
                } else
                {
                    Common.message(string.Format("MESSAGE {0}: {1}", e.ChatMessage.DisplayName, e.ChatMessage.Message));
                }
            }
            ChatFiltering.violatesProtections(e.ChatMessage.Username, Common.isSub(e), Common.isMod(e), e.ChatMessage.Message);
            if (Common.AhoyRewarder.isActive())
                Common.AhoyRewarder.processMessage(e);
            processPotentialSub(e);
            Common.ChatMessageTracker.addMessage(e.ChatMessage);
        }

        public static void chatOnCommand(object sender, TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            ChatFiltering.violatesProtections(e.ChatMessage.Username, Common.isSub(e), Common.isMod(e), e.ChatMessage.Message);
            Commands.handleChatCommand(e);
            processPotentialSub(e);
            Common.ChatMessageTracker.addMessage(e.ChatMessage);
        }

        public static void chatOnSubscribe(object sender, TwitchLib.TwitchChatClient.NewSubscriberArgs e)
        {
            Common.other(string.Format("SUBSCRIBER {0}, months: ", e.Subscriber.Months));
            Subscriptions.handleSubscription(e);
        }

        public static void whisperOnConnected(object sender, TwitchLib.TwitchWhisperClient.OnConnectedArgs e)
        {
            Common.success(string.Format("[WHISPER]Connected to group chat!"));
        }

        public static void whisperOnWhisper(object sender, TwitchLib.TwitchWhisperClient.NewWhisperReceivedArgs e)
        {
            if(e.WhisperMessage.Message[0] != '!')
                Common.other(string.Format("WHISPER {0}: {1}", e.WhisperMessage.DisplayName, e.WhisperMessage.Message));
        }

        public static void whisperOnCommand(object sender, TwitchLib.TwitchWhisperClient.CommandReceivedArgs e)
        {
            Common.command(string.Format("[WHISPER]COMMAND {0}: {1}", e.Username, e.Command + e.ArgumentsAsString), true);
            Commands.handleWhisperCommand(e);
        }

        public static void raidOnMessage(object sender, TwitchLib.TwitchChatClient.NewChatMessageArgs e)
        {
            Common.other(string.Format("[RAID]MESSAGE {0}: {1}", e.ChatMessage.DisplayName, e.ChatMessage.Message));
            if (Common.RaidInstance != null)
                Common.RaidInstance.handleMessage(e);
        }

        public static void onDiscordConnected(object sender, DiscordConnectEventArgs e)
        {
            Common.DiscordClient.UpdateCurrentGame("BurkeBlack.TV");
            Common.DiscordClient.SendMessageToChannel("connected!", Common.DiscordClient.GetChannelByName("kraken-relay"));
        }

        public static void onDiscordMessageReceived(object sender, DiscordMessageEventArgs e)
        {
            if (e.message_text.Length > 0 && e.message_text[0] == '!')
                Commands.handleDiscordCommand(new Objects.DiscordCommand(e.author.Username, e.message_text, e.Channel.Name));
            if(e.Channel.Name.ToLower() == "kraken-relay")
            {
                if (e.message.content.ToLower() == "!restart")
                {
                    Common.DiscordClient.SendMessageToChannel("Restarting... Please standby!", Common.DiscordClient.GetChannelByName("kraken-relay"));
                    System.Diagnostics.Process.Start(Assembly.GetExecutingAssembly().Location);
                    Environment.Exit(0);
                }
            }
            Console.WriteLine(string.Format("[{0}] {1}: {2}", e.Channel.Name, e.author.Username, e.message_text));
        }

        private static void processPotentialSub(TwitchLib.TwitchChatClient.NewChatMessageArgs e)
        {
            if (e.ChatMessage.Subscriber && !Common.ChatSubs.Contains(e.ChatMessage.Username.ToLower()))
                Common.ChatSubs.Add(e.ChatMessage.Username.ToLower());
        }

        private static void processPotentialSub(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (e.ChatMessage.Subscriber && !Common.ChatSubs.Contains(e.ChatMessage.Username.ToLower()))
                Common.ChatSubs.Add(e.ChatMessage.Username.ToLower());
        }
    }
}
