using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;

namespace KrakenBot2
{
    public static class Update
    {

        public static bool checkForPostUpdate()
        {
            return File.Exists("updater.exe");
        }

        public static void processUpdateRequest()
        {
            UpdateDetails details = WebCalls.downloadUpdateDetails().Result;
            if (!details.RequestSuccessful || !details.UpdateAvailable)
            {
                Console.WriteLine("RequestSuccessful: " + details.RequestSuccessful + ", UpdateAvailable: " + details.UpdateAvailable);
                return;
            }
            Console.WriteLine("Downloading new bot and updater...");
            WebCalls.downloadFile(details.DownloadLocation, "bot.exe");
            Console.WriteLine("new bot downloaded");
            WebCalls.downloadFile(details.UpdaterLocation, "updater.exe");
            Console.WriteLine("Finished! Starting updater and exiting.");
            System.Diagnostics.ProcessStartInfo pInfo = new System.Diagnostics.ProcessStartInfo();
            pInfo.FileName = "updater.exe";
            pInfo.ErrorDialog = true;
            pInfo.UseShellExecute = false;
            pInfo.RedirectStandardOutput = true;
            pInfo.RedirectStandardError = true;
            pInfo.WorkingDirectory = Path.GetDirectoryName("updater.exe");
            System.Diagnostics.Process.Start(pInfo);
            Environment.Exit(1);
        }

        public static void afterUpdate()
        {
            if(File.Exists("updater.exe"))
            {
                File.Delete("updater.exe");
                Common.UpdateDatas = new Common.UpdateData(WebCalls.downloadUpdateDetails().Result);
                Common.notify("Update successful!", "Changes: " + Common.UpdateDatas.Details.Changes);
                Console.WriteLine("Waiting for connected state...");
                while(!Events.connected) { }
                Console.WriteLine("Connected to chat confirmed!");
                if (Common.UpdateDatas.Details.Announce)
                    Common.ChatClient.sendMessage(string.Format("Updated! Changes: {0}", Common.UpdateDatas.Details.Changes), Common.DryRun);
            }
        }
    }

    public class UpdateDetails {
        private bool requestSuccessful = false;
        private bool updateAvailable = false;
        private bool announce = false;
        private string changes;
        private string updaterLocation;
        private string downloadLocation;

        public bool Announce { get { return announce; } }
        public bool RequestSuccessful { get { return requestSuccessful; } }
        public bool UpdateAvailable { get { return updateAvailable; } }
        public string Changes { get { return changes; } }
        public string DownloadLocation { get { return downloadLocation; } }
        public string UpdaterLocation { get { return updaterLocation; } }

        public UpdateDetails(JToken updateData)
        {
            if (updateData.SelectToken("update_request").ToString() == "successful")
                requestSuccessful = true;
            if (updateData.SelectToken("update_available").ToString() == "true")
                updateAvailable = true;
            changes = updateData.SelectToken("changes").ToString();
            updaterLocation = updateData.SelectToken("updater_link").ToString();
            downloadLocation = updateData.SelectToken("kraken_link").ToString();
            if (updateData.SelectToken("announce").ToString() == "true")
                announce = true;
        }
    }
}
