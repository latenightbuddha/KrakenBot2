using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.HardCodedChatCommands
{
    public static class Permit
    {
        public static void handleCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (verifyCommand(e))
            {
                string receiver = e.ArgumentsAsList[0].ToLower();
                int usages = 1;
                if (e.ArgumentsAsList.Count == 2)
                    usages = int.Parse(e.ArgumentsAsList[1]);
                bool found = false;
                foreach(Objects.Permit oldPermit in Common.Permits)
                {
                    if(oldPermit.Username.ToLower() == receiver)
                    {
                        found = true;
                        oldPermit.update(DateTime.Now, usages);
                    }
                }
                if (!found)
                    Common.Permits.Add(new Objects.Permit(receiver, DateTime.Now, usages));
                if (usages == 1)
                    Common.ChatClient.sendMessage(string.Format("You are permitted to post 1 link, {0}!", receiver), Common.DryRun);
                else
                    Common.ChatClient.sendMessage(string.Format("You are permitted to post {0} links, {1}", usages, receiver), Common.DryRun);
                Common.command(e.Command, true);
            } else
            {
                Common.command(e.Command, false);
            }
        }

        private static bool verifyCommand(TwitchLib.TwitchChatClient.CommandReceivedArgs e)
        {
            if (!Common.Cooldown.chatCommandAvailable(e.ChatMessage.UserType, e.Command, 0))
                return false;
            if (e.ArgumentsAsList.Count < 1 && e.ArgumentsAsList.Count > 2)
                return false;
            if (e.ArgumentsAsList.Count == 2 && !Common.IsNumeric(e.ArgumentsAsList[1]))
                return false;
            if (!Common.isMod(e))
                return false;
            if (Common.DryRun)
                return false;
            return true;
        }
    }
}
