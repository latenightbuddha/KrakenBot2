using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace KrakenBot2
{
    public class DoubloonDistributor
    {
        // Doubloon distrobution interval: online = 15 minutes, offline = 1 hour
        Timer onlineAllocator = new Timer(900000);
        Timer offlineAllocator = new Timer(3600000);

        // DoubloonDistributor constructor
        public DoubloonDistributor()
        {
            onlineAllocator.Elapsed += onlineAllocatorTick;
            offlineAllocator.Elapsed += offlineAllocatorTick;
            if (Common.StreamRefresher.isOnline())
                onlineAllocator.Start();
            else
                offlineAllocator.Start();
        }

        // Method to set both timers to an online environment
        public void forceOnline()
        {
            offlineAllocator.Stop();
            onlineAllocator.Start();
        }

        // Method to set both timers to an offline environment
        public void forceOffline()
        {
            onlineAllocator.Stop();
            offlineAllocator.Start();
        }

        // Online timer tick event
        public void onlineAllocatorTick(object sender, ElapsedEventArgs e)
        {
            if(!Common.StreamRefresher.isOnline())
            {
                onlineAllocator.Stop();
                offlineAllocator.Start();
            }
            if (!WebCalls.distibuteDoubloons(1).Result)
                Common.ChatClient.sendMessage("Failed to distribute doubloons.");
        }

        // Offline timer tick event
        public void offlineAllocatorTick(object sender, ElapsedEventArgs e)
        {
            if(Common.StreamRefresher.isOnline())
            {
                offlineAllocator.Stop();
                onlineAllocator.Start();
            }
            if (!WebCalls.distibuteDoubloons(1).Result)
                Common.ChatClient.sendMessage("Failed to distributre doubloons.");
        }
    }
}
