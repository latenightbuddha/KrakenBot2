using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace KrakenBot2
{
    public static class ChatFiltering
    {
        private static string[] capsMessages = { "Stop Yelling!", "What are you yelling for!", "You are yelling? WHY!?", "You will not yell in my chat!", "That's a lot of capital letters!" };
        private static string[] spamMessages = { "What... What is that!?", "Was that English, or a bunch of spaaaaam?", "That's a whole lot of letters, with a whole little meaning!", "Made up words? Yes! Made up words!",
                                                   "No one even knows what that means!" };
        private static string[] emoteMessages = { "You. Shall. Not. Emote (that much)!", "One emote. Two emote. Too many emote. Purge!", "Lots of small images. Too many small images!!", "Emotes are not currency.  You are not rich!",
                                                    "Human error leads to too many emotes!!"};
        private static string[] lengthMessages = { "That message.  It is quite long.", "That is quite the novel!", "Wall of text? I think not!", "Did you mean to send that HUGE message?", "TL;DR pls." };
        private static string[] timeoutWordMessages = { "Woooaaah, easy there chap.", "Some words I just can't allow!", "Burke wouldn't like that word in chat.", "PG-16 means not using that bad word!!", "Bad language = purge :(" };
        private static string[] spoilerWordMessages = { "This has all the signs of a spoiler!", "Spoil you shall not!", "You mustn't spoil. You mustn't spoil.", "No story details!!!", "And then there was no spoilers!" };
        private static string[] linkMessages = { "Link you shall not!", "Sub status: False. Link status: Purged.", "Once upon a time THERE WAS NO LINK!!", "Just when you thought you could link...", "Your link will not see the light of day!!" };

        //Returns true if 
        private static int minimumCharacters = 5;
        private static int capsPercentage = 75;
        private static bool violateCapsProtection(string message)
        {
            if(message.Length >= minimumCharacters)
            {
                int totalChars = 0;
                int capsCount = 0;
                foreach (char character in message.ToCharArray())
                {
                    if (character != ' ')
                    {
                        totalChars++;
                        if (char.IsUpper(character))
                            capsCount++;
                    }
                }
                if ((((double)capsCount / totalChars) * 10) > capsPercentage)
                    return true;
            }
            return false;
        }

        //Returns true if words are too long
        private static int longestWord = 26;
        private static bool violateSpamProtection(string message)
        {
            foreach(string word in message.Split(' '))
            {
                if (word.Length > longestWord)
                    return true;
            }
            return false;
        }

        //returns true if emote count in message is larger than that of the limit passed in
        private static int emoteLimit = 8;
        private static bool violateEmoteProtection(List<string> emotes, string message)
        {
            message = message.ToLower();
            int emoteCount = 0;
            foreach(string word in message.Split(' '))
            {
                if(emotes != null)
                    foreach(string emote in emotes)
                        if(word.ToLower() == emote.ToLower())
                            emoteCount++;
            }
            if (emoteCount > emoteLimit)
                return true;
            return false;
        }

        //Returns true if message length is larger than limit passed in
        private static int lengthLimit = 350;
        private static bool violateLengthProtection(string message)
        {
            return (message.Length > lengthLimit);
        }

        //Returns true if timeout word found in message
        private static Objects.TimeoutWord violateTimeoutWordsProtection(List<Objects.TimeoutWord> words, string message)
        {
            foreach (Objects.TimeoutWord word in words)
            {
                List<string> violationWords = new List<string>();
                if (word.Word.Contains('|'))
                    foreach (string vioWord in word.Word.Split('|'))
                        violationWords.Add(vioWord.ToLower());
                else
                    violationWords.Add(word.Word.ToLower());
                if (message.Contains(' '))
                {
                    foreach (string messageWord in message.Split(' '))
                        if (messageContainsViolation(violationWords, messageWord))
                            return word;
                } else
                {
                    if (messageContainsViolation(violationWords, message))
                        return word;
                }
            }
            return null;
        }

        //Returns true if spoiler word found in message
        private static Objects.SpoilerWord violateSpoilerWordsProtection(List<Objects.SpoilerWord> words, string message)
        {
            foreach (Objects.SpoilerWord word in words)
            {
                List<string> violationWords = new List<string>();
                if (word.Word.Contains('|'))
                    foreach (string vioWord in word.Word.Split('|'))
                        violationWords.Add(vioWord.ToLower());
                else
                    violationWords.Add(word.Word.ToLower());
                if (message.Contains(' '))
                {
                    foreach (string messageWord in message.Split(' '))
                        if (messageContainsViolation(violationWords, messageWord))
                            return word;
                }
                else
                {
                    if (messageContainsViolation(violationWords, message))
                        return word;
                }
            }
            return null;
        }

        private static bool messageContainsViolation(List<string> violationWords, string message)
        {
            foreach (string vioWord in violationWords)
                if (message.ToLower() == vioWord.ToLower())
                    return true;
            return false;
        }

        //Returns true if link is detected in message
        private static bool violateLinkProtection(string message)
        {
            foreach(string word in message.Split(' '))
            {
                if(word.Contains('.'))
                {
                    string[] parts = word.Split('.');
                    if(parts[parts.Length - 1].Contains('/'))
                        if (Common.TopLevelDomains.Contains(parts[parts.Length - 1].Split('/')[0]))
                            return true;
                    else
                        if (Common.TopLevelDomains.Contains(parts[parts.Length - 1]) && parts[0].Length > 0)
                        return true;
                    
                }
            }
            return false;
        }

        public static bool violatesProtections(string username, bool sub, bool mod, string message)
        {
            if (username != "the_kraken_bot" && username != "swiftyspiffy" && message.ToLower().Contains("swifty"))
                Common.notify("CHAT FLAG - SWIFTY", username + ": " + message);
            if(!sub)
            {
                if(violateCapsProtection(message))
                {
                    Common.ChatClient.sendMessage(string.Format("[{0}] {1} [{2}] *warning*", username, capsMessages[new Random().Next(0, capsMessages.Length)], "Please don't use so many caps."), Common.DryRun);
                    Common.WhisperClient.sendWhisper(username, "Please don't use so many capital letters. *warned*", Common.DryRun);
                    System.Threading.Thread.Sleep(400);
                    Common.ChatClient.sendMessage(string.Format(".timeout {0} 1", username), Common.DryRun);
                    Common.rep(string.Format("CAPS TIMEOUT: user: {0}, message: {1}", username, message));
                    Common.notify("CHAT FILTER - CAPS PROTECTION", "User: " + username + ", message: " + message);
                    Common.DiscordClient.SendMessageToChannel(string.Format("TIMEOUT (user: {0}): Violation: CAPS, Message: {1}", username, message), Common.DiscordClient.GetChannelByName("kraken-relay"));
                    return true;
                }
                if(violateSpamProtection(message) && !violateLinkProtection(message) && !message.Contains("PRIVMSG"))
                {
                    Common.ChatClient.sendMessage(string.Format("[{0}] {1} [{2}] *warning*", username, spamMessages[new Random().Next(0, spamMessages.Length)], "Please don't use so much spam."), Common.DryRun);
                    Common.WhisperClient.sendWhisper(username, "Please don't use so much spam. *warned*", Common.DryRun);
                    System.Threading.Thread.Sleep(400);
                    Common.ChatClient.sendMessage(string.Format(".timeout {0} 1", username), Common.DryRun);
                    Common.rep(string.Format("SPAM TIMEOUT: user: {0}, message: {1}", username, message));
                    Common.notify("CHAT FILTER - SPAM PROTECTION", "User: " + username + ", message: " + message);
                    Common.DiscordClient.SendMessageToChannel(string.Format("TIMEOUT (user: {0}): Violation: SPAM, Message: {1}", username, message), Common.DiscordClient.GetChannelByName("kraken-relay"));
                    return true;
                }
                if(violateEmoteProtection(Common.CachedEmotes, message))
                {
                    Common.ChatClient.sendMessage(string.Format("[{0}] {1} [{2}] *warning*", username, emoteMessages[new Random().Next(0, emoteMessages.Length)], "Please don't use so many emotes."), Common.DryRun);
                    Common.WhisperClient.sendWhisper(username, "Please don't use so many emotes. *warned*", Common.DryRun);
                    System.Threading.Thread.Sleep(400);
                    Common.ChatClient.sendMessage(string.Format(".timeout {0} 1", username), Common.DryRun);
                    Common.rep(string.Format("EMOTE TIMEOUT: user: {0}, message: {1}", username, message));
                    Common.notify("CHAT FILTER - EMOTE PROTECTION", "User: " + username + ", message: " + message);
                    Common.DiscordClient.SendMessageToChannel(string.Format("TIMEOUT (user: {0}): Violation: EMOTES, Message: {1}", username, message), Common.DiscordClient.GetChannelByName("kraken-relay"));
                    return true;
                }
                if(violateLengthProtection(message) && !message.Contains("PRIVMSG"))
                {
                    Common.ChatClient.sendMessage(string.Format("[{0}] {1} [{2}] *warning*", username, lengthMessages[new Random().Next(0, lengthMessages.Length)], "Please don't make your messages so long."), Common.DryRun);
                    Common.WhisperClient.sendWhisper(username, "Please don't make your messages so long. *warned*", Common.DryRun);
                    System.Threading.Thread.Sleep(400);
                    Common.ChatClient.sendMessage(string.Format(".timeout {0} 1", username), Common.DryRun);
                    Common.rep(string.Format("LENGTH TIMEOUT: user: {0}, message: {1}", username, message));
                    Common.notify("CHAT FILTER - LENGTH PROTECTION", "User: " + username + ", message(length: " + message.Length + "): " + message);
                    Common.DiscordClient.SendMessageToChannel(string.Format("TIMEOUT (user: {0}): Violation: LENGTH, Message: {1}", username, message), Common.DiscordClient.GetChannelByName("kraken-relay"));
                    return true;
                }
                if(violateLinkProtection(message) && !linkPermitExists(username))
                {
                    Common.ChatClient.sendMessage(string.Format("[{0}] {1} [{2}] *warning*", username, linkMessages[new Random().Next(0, linkMessages.Length)], "Please get permission for sending links."), Common.DryRun);
                    Common.WhisperClient.sendWhisper(username, "Please get permission for sending links. *warned*", Common.DryRun);
                    System.Threading.Thread.Sleep(400);
                    Common.ChatClient.sendMessage(string.Format(".timeout {0} 1", username), Common.DryRun);
                    Common.rep(string.Format("LINK TIMEOUT: user: {0}, message: {1}", username, message));
                    Common.notify("CHAT FILTER - LINK PROTECTION", "User: " + username + ", message: " + message);
                    Common.DiscordClient.SendMessageToChannel(string.Format("TIMEOUT (user: {0}): Violation: LINK, Message: {1}", username, message), Common.DiscordClient.GetChannelByName("kraken-relay"));
                    return true;
                }
            }
            if(!mod)
            {
                Objects.TimeoutWord toWord = violateTimeoutWordsProtection(Common.TimeoutWords, message);
                if(toWord != null)
                {
                    Common.ChatClient.sendMessage(string.Format("[{0}] {1} [{2}] *warning*", username, timeoutWordMessages[new Random().Next(0, timeoutWordMessages.Length)], "Please don't send religious, poltical, or obscene messages."), Common.DryRun);
                    Common.WhisperClient.sendWhisper(username, "Please don't send religious, poltical, or obscene messages. *warned*", Common.DryRun);
                    System.Threading.Thread.Sleep(400);
                    Common.ChatClient.sendMessage(string.Format(".timeout {0} {1}", username, toWord.Seconds), Common.DryRun);
                    Common.rep(string.Format("BAD WORD TIMEOUT: user: {0}, word: {1}, message: {2}", username, toWord.Word, message));
                    Common.notify("CHAT FILTER - BAD WORD PROTECTION", "User: " + username + ", word: " + toWord.Word + "message: " + message);
                    Common.DiscordClient.SendMessageToChannel(string.Format("TIMEOUT (user: {0}): Violation: BAD WORD, Message: {1}", username, message), Common.DiscordClient.GetChannelByName("kraken-relay"));
                    return true;
                }
                Objects.SpoilerWord spoilerWord = violateSpoilerWordsProtection(Common.SpoilerWords, message);
                if(spoilerWord != null)
                {
                    Common.ChatClient.sendMessage(string.Format("[{0}] {1} [{2}] *warning*", username, spoilerWordMessages[new Random().Next(0, spoilerWordMessages.Length)], "Please don't include potential story spoilers in your message."), Common.DryRun);
                    Common.WhisperClient.sendWhisper(username, "Please don't include potential story spoilers in your message. *warned*", Common.DryRun);
                    System.Threading.Thread.Sleep(400);
                    Common.ChatClient.sendMessage(string.Format(".timeout {0} {1}", username, spoilerWord.Seconds), Common.DryRun);
                    Common.rep(string.Format("SPOILER TIMEOUT: user: {0}, word: {1}, message: {2}", username, spoilerWord.Word, message));
                    Common.notify("CHAT FILTER - SPOILER WORD PROTECTION", "User: " + username + ", word: " + spoilerWord.Word + "message: " + message);
                    Common.DiscordClient.SendMessageToChannel(string.Format("TIMEOUT (user: {0}): Violation: SPOILER WORD, Message: {1}", username, message), Common.DiscordClient.GetChannelByName("kraken-relay"));
                    return true;
                }
            }
            
            return false;
        }

        private static bool linkPermitExists(string username)
        {
            foreach(Objects.Permit permit in Common.Permits)
                if(permit.Username.ToLower() == username.ToLower())
                    return permit.usePermit();
            return false;
        }
    }
}
