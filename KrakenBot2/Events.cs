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
            connected = true;
            if (!Common.EntryMessage)
                return;
            Common.ChatClient.sendMessage(string.Format("/me [V2] connected[v{0}]!", Assembly.GetExecutingAssembly().GetName().Version), Common.DryRun);
        }

        public static void chatOnConnect(object sender, TwitchLib.TwitchChatClient.OnConnectedArgs e)
        {
            Common.success("[CHAT]Connected to channel!");
        }

        public static void chatOnMessage(object sender, TwitchLib.TwitchChatClient.NewChatMessageArgs e)
        {
            if(e.ChatMessage.Message[0] != '!')
                Common.rep(string.Format("MESSAGE {0}: {1}", e.ChatMessage.DisplayName, e.ChatMessage.Message));
            ChatFiltering.violatesProtections(e.ChatMessage.Username, Common.isSub(e), Common.isMod(e), e.ChatMessage.Message);
            if (Common.AhoyRewarder.isActive())
                Common.AhoyRewarder.processMessage(e);
            Common.ChatMessageTracker.addMessage(e.ChatMessage);
        }

        public static void chatOnCommand(object sender, TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            Common.rep(string.Format("[CHAT]COMMAND {0}: {1}", e.ChatMessage.Message, e.Command + e.ArgumentsAsString));
            ChatFiltering.violatesProtections(e.ChatMessage.Username, Common.isSub(e), Common.isMod(e), e.ChatMessage.Message);
            Commands.handleChatCommand(e);
            Common.ChatMessageTracker.addMessage(e.ChatMessage);
        }

        public static void chatOnSubscribe(object sender, TwitchLib.TwitchChatClient.NewSubscriberArgs e)
        {
            Common.rep(string.Format("SUBSCRIBER {0}, months: ", e.Subscriber.Months));
            Subscriptions.handleSubscription(e);
        }

        public static void whisperOnConnected(object sender, TwitchLib.TwitchWhisperClient.OnConnectedArgs e)
        {
            Common.success(string.Format("[WHISPER]Connected to group chat!"));
        }

        public static void whisperOnWhisper(object sender, TwitchLib.TwitchWhisperClient.NewWhisperReceivedArgs e)
        {
            if(e.WhisperMessage.Message[0] != '!')
                Common.rep(string.Format("WHISPER {0}: {1}", e.WhisperMessage.DisplayName, e.WhisperMessage.Message));
        }

        public static void whisperOnCommand(object sender, TwitchLib.TwitchWhisperClient.CommandReceivedArgs e)
        {
            Common.rep(string.Format("[WHISPER]COMMAND {0}: {1}", e.Username, e.Command + e.ArgumentsAsString));
            Commands.handleWhisperCommand(e);
        }

        public static void raidOnMessage(object sender, TwitchLib.TwitchChatClient.NewChatMessageArgs e)
        {
            Common.rep(string.Format("[RAID]MESSAGE {0}: {1}", e.ChatMessage.DisplayName, e.ChatMessage.Message));
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
                Commands.handleDiscordCommand(new Objects.DiscordCommand(e.author.user.username, e.message_text, e.Channel.name));
            Console.WriteLine(string.Format("[{0}] {1}: {2}", e.Channel.name, e.author.user.username, e.message_text));
        }

        public static void onDiscordPrivateMessageReceived(object sender, DiscordPrivateMessageEventArgs e)
        {
            //if (e.message[0] == '!')
            //Commands.handleDiscordCommand(new Objects.DiscordCommand(e.author.user.username, e.message, "", true));
        }
    }
}
