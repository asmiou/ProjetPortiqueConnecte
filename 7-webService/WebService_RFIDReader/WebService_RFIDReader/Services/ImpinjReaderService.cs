using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Impinj.OctaneSdk;


namespace WebService_RFIDReader.Services
{

    public class ImpinjReaderService
    {
        static ImpinjReader reader = new ImpinjReader();
        Models.Reader myReader;
        Models.Antenna myAntenna;
        static string delimit = ";";
        public string prefixPath { set; get; }

        public static List<string[]> listTags = new List<string[]>();

        public ImpinjReaderService(Models.Reader r, Models.Antenna a)
        {
            myReader = r;
            myAntenna = a;
        }

        private void configureReader()
        {
            try
            {
                Settings settings = reader.QueryDefaultSettings();
                settings.AutoStart.Mode = AutoStartMode.Immediate;
                settings.AutoStop.Mode = AutoStopMode.None;
                settings.Gpos.GetGpo(1).Mode = GpoMode.LLRPConnectionStatus;
                settings.Report.IncludeAntennaPortNumber = true;
                settings.ReaderMode = ReaderMode.AutoSetDenseReader;
                settings.SearchMode = SearchMode.DualTarget;
                settings.Session = myReader.session;
                setAntennas(settings);

                settings.Report.IncludeFirstSeenTime = true;
                settings.Report.IncludeLastSeenTime = true;
                settings.Report.IncludeSeenCount = true;
                settings.Report.IncludePeakRssi = true;

                settings.HoldReportsOnDisconnect = true;

                settings.Keepalives.Enabled = true;
                settings.Keepalives.PeriodInMs = 5000;

                settings.Keepalives.EnableLinkMonitorMode = true;
                settings.Keepalives.LinkDownThreshold = 5;

                //reader.KeepaliveReceived += OnKeepaliveReceived;

                reader.ConnectionLost += OnConnectionLost;
                reader.ApplySettings(settings);
                reader.SaveSettings();
                reader.TagsReported += OnTagsReported;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void setAntennas(Settings settings)
        {
            try
            {
                settings.Antennas.EnableAll();
                for (int i = 0; i <= myReader.antennas; i++)
                {
                    ushort num = 1;
                    settings.Antennas.GetAntenna(num).TxPowerInDbm = myAntenna.power;
                    settings.Antennas.GetAntenna(num).RxSensitivityInDbm = myAntenna.sensibility;
                    num++;
                }

            }
            catch (Exception e)
            {
                throw e;
                //Console.WriteLine("Erreur in Enable All: " + e.ToString());
            }
        }

        public void ConnectToReader()
        {
            try
            {
                reader.Name = "Reader Impinj R420 N#-" + myReader.readerId;
                reader.ConnectTimeout = myReader.connectTimeout;
                reader.Connect(myReader.ipAdress);
                reader.ResumeEventsAndReports();
            }
            catch (OctaneSdkException e)
            {
                throw e;
            }
        }

        private void OnConnectionLost(ImpinjReader reader)
        {
            reader.Disconnect();
            ConnectToReader();
        }

        private void OnKeepaliveReceived(ImpinjReader reader)
        {
            // Console.WriteLine("Keepalive received from {0} ({1})", reader.Name, reader.Address);
        }

        static void OnTagsReported(ImpinjReader sender, TagReport report)
        {
            foreach (Tag tag in report)
            {
                //Models.TagRead t = new Models.TagRead(tag.LastSeenTime.ToString(), tag.Epc.ToString(), tag.PeakRssiInDbm, tag.AntennaPortNumber); 
                //listTags.Add(t);
                string[] t = { tag.Epc + delimit + tag.PeakRssiInDbm + delimit + tag.LastSeenTime + delimit + tag.AntennaPortNumber };
                listTags.Add(t);

            }
        }

        private void saveOnfile()
        {
            //string today = DateTime.Now.ToString().Replace('/', '_').Replace(':', '-').Replace(' ', 'H');
            string today = DateTime.Now.ToString("yyyyMMddHHmmss");
            string appDataFolder = HttpContext.Current.Server.MapPath("~/App_Data/1-RawData/");

            string filename = "scan";
            string filepath = appDataFolder + today + "_" + filename + ".csv";

            try
            {
                var file = File.CreateText(filepath);
                foreach (var t in listTags)
                {
                    file.WriteLine(string.Join(delimit, t));
                }
                prefixPath = today;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void stopScan()
        {
            Console.WriteLine("Stoped");
            reader.Stop();
            saveOnfile();
            reader.Disconnect();
        }

        public void startScan()
        {
            Console.WriteLine("Started");
            //ConnectToReader();
            configureReader();
        }
    }
}
