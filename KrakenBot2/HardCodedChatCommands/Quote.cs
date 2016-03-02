using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Quote
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                if(e.ArgumentsAsList.Count == 0)
                {
                    //Random quote
                    Objects.Quote randomQuote = Common.Quotes[new Random().Next(0, Common.Quotes.Count - 1)];
                    Common.ChatClient.sendMessage(string.Format("[{0}/{1}] {2} - {3}", randomQuote.ID, Common.Quotes[Common.Quotes.Count - 1].ID, randomQuote.QuoteContents, randomQuote.Author), Common.DryRun);
                } else
                {
                    //Indexed quote
                    if(int.Parse(e.ArgumentsAsList[0]) <= Common.Quotes[Common.Quotes.Count - 1].ID)
                    {
                        foreach(Objects.Quote quote in Common.Quotes)
                            if(quote.ID.ToString() == e.ArgumentsAsList[0])
                            {
                                Common.ChatClient.sendMessage(string.Format("[{0}/{1}] {2} - {3}", quote.ID, Common.Quotes[Common.Quotes.Count - 1].ID, quote.QuoteContents, quote.Author), Common.DryRun);
                                return;
                            }
                        Common.ChatClient.sendMessage("Invalid quote index [quote deleted]!", Common.DryRun);
                    } else
                    {
                        Common.ChatClient.sendMessage("Invalid quote index [too large]!", Common.DryRun);
                    }
                }
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (!Common.Cooldown.chatCommandAvailable(e.ChatMessage.UserType, e.Command, 10))
                return false;
            if (e.ArgumentsAsList.Count > 1)
                return false;
            if (e.ArgumentsAsList.Count == 1 && !Common.IsNumeric(e.ArgumentsAsList[0]))
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
