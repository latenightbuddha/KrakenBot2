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
        Timer onlineAllocator = new Timer(900000);
        Timer offlineAllocator = new Timer(3600000);

        public DoubloonDistributor()
        {
            onlineAllocator.Elapsed += onlineAllocatorTick;
            offlineAllocator.Elapsed += offlineAllocatorTick;
            if (Common.StreamRefresher.isOnline())
                onlineAllocator.Start();
            else
                offlineAllocator.Start();
        }

        public void forceOnline()
        {
            offlineAllocator.Stop();
            onlineAllocator.Start();
        }

        public void forceOffline()
        {
            onlineAllocator.Stop();
            offlineAllocator.Start();
        }

        public void onlineAllocatorTick(object sender, ElapsedEventArgs e)
        {
            if(Common.StreamRefresher.isOffline())
            {
                onlineAllocator.Stop();
                offlineAllocator.Start();
            }
            WebCalls.distibuteDoubloons(1);
        }

        public void offlineAllocatorTick(object sender, ElapsedEventArgs e)
        {
            if(Common.StreamRefresher.isOnline())
            {
                offlineAllocator.Stop();
                onlineAllocator.Start();
            }
            WebCalls.distibuteDoubloons(1);
        }
    }
}
