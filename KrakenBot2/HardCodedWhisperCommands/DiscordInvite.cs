using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedWhisperCommands
{
    public static class DiscordInvite
    {
        public static void handleCommand(TwitchLib.TwitchWhisperClient.CommandReceivedArgs e)
        {
            if (Common.Cooldown.chatCommandAvailable(TwitchLib.ChatMessage.uType.Viewer, e.Command, 20))
            {
                if (int.Parse(WebCalls.getUserDoubloons(e.Username).Result) > Common.DiscordInviteLimit)
                {
                    string oldInvite = WebCalls.getExistingDiscordInvite(e.Username).Result;
                    if (oldInvite != null)
                    {
                        if (Common.WhisperClient != null)
                            Common.WhisperClient.sendWhisper(e.Username, string.Format("It appears you've already requested an invite.  You may try accessing your old invite here (https://discord.gg/{0}), but it may be invalid.", oldInvite), Common.DryRun);
                    }
                    else
                    {
                        //string invite = WebCalls.createInviteCode();
                        //if (Common.WhisperClient != null)
                        //    Common.WhisperClient.sendWhisper(e.Username, string.Format("Here is your BurkeBlack Crew Discord Chat invite.  You have two 2 minutes, and this link will expire after its first usage.  You will not receive another link. https://discord.gg/{0}", invite), Common.DryRun);
                        //WebCalls.addInviteCode(e.Username, invite);
                    }
                }
                else
                {
                    if (Common.WhisperClient != null)
                        Common.WhisperClient.sendWhisper(e.Username, string.Format("You currently do not meet the {0} doubloon limit.  Please request an invite when you have met this requirement.", Common.DiscordInviteLimit), Common.DryRun);
                }
            } else
            {
                if (Common.WhisperClient != null)
                    Common.WhisperClient.sendWhisper(e.Username, string.Format("There is a 20 second cooldown on this whisper command applied to everyone whispering me (The_Kraken_Bot).  Please try again shortly."));
            }
        }
    }
}
