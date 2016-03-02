using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2
{
    // Static class with methods relating to subscription event
    public static class Subscriptions
    {
        // Configurable variables
        private static int newSubDoubloons = 100;
        private static int newSubSoundbytes = 5;
        private static int resubDoubloons = 50;
        private static int resubSoundbytes = 5;

        // Handles subscription event
        public static void handleSubscription(TwitchLib.TwitchChatClient.NewSubscriberArgs e)
        {
            Common.RecentSub = e.Subscriber;
            if (e.Subscriber.Months > 0)
            {
                WebCalls.addSoundbyteCredits(e.Subscriber.Name, resubSoundbytes);
                WebCalls.addDoubloons(e.Subscriber.Name, resubDoubloons);
                Common.ChatClient.sendMessage(string.Format("Welcome back returning crewmember {0}, of {1} months!!! burkeAhoy burkeFlag burkeAhoy burkeFlag", 
                    e.Subscriber.Name, e.Subscriber.Months), Common.DryRun);
                Common.ChatClient.sendMessage(string.Format("Enjoy your resub booty of {0} doubloons and {1} sound byte credits, {2}!",
                    resubDoubloons, resubSoundbytes, e.Subscriber.Name), Common.DryRun);
            } else
            {
                WebCalls.addSoundbyteCredits(e.Subscriber.Name, newSubSoundbytes);
                WebCalls.addDoubloons(e.Subscriber.Name, newSubDoubloons);
                Common.ChatClient.sendMessage(string.Format("Welcome aboard matey! Please welcome the latest crewmember to the BurkeBlack crew, {0}! burkeAhoy burkeFlag burkeAhoy burkeFlag", e.Subscriber.Name), Common.DryRun);
                Common.ChatClient.sendMessage(string.Format("Enjoy your subscription booty of 100 doubloons and 5 sound byte credits, {0}!", e.Subscriber.Name), Common.DryRun);
            }
        }
    }
}
