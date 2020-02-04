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
        Settings settings = reader.QueryDefaultSettings();
        public static List<Models.TagRead> listTags = new List<Models.TagRead>();
        
        public ImpinjReaderService(Models.Reader r){
            this.myReader = r;
            //this.configureReader();
            //this.connectReader();
        }

        private void ConnectToReader(){
            try
            {
                reader.ConnectTimeout = myReader.connectTimeout;
                reader.Connect(myReader.ipAdress);
                reader.ResumeEventsAndReports();
            }
            catch (OctaneSdkException e)
            {
                throw e;
            }
        }

        private void OnConnectionLost(ImpinjReader reader){
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
                //string[] t = { tag.Epc + "," + tag.PeakRssiInDbm + "," + tag.LastSeenTime + "," + tag.AntennaPortNumber };
                Models.TagRead t = new Models.TagRead(tag.LastSeenTime.ToString(), tag.Epc.ToString(), tag.PeakRssiInDbm, tag.AntennaPortNumber); 
                listTags.Add(t);
            }
        }

        private void saveOnfile(){
            string today = DateTime.Now.ToString().Replace('/', '_').Replace(':', '-').Replace(' ', 'H');
            string appDataFolder = HttpContext.Current.Server.MapPath("~/App_Data/1-RawData/");

            string filename = "scan";
            string filepath = appDataFolder + filename + "-" + today + ".csv";
            string delimit = ",";

            var file = File.CreateText(filepath);
            foreach (var t in listTags)
            {
                string line = t.ecp + delimit + t.rssi + delimit + t.timesTamp + delimit + t.antenna;
                file.WriteLine(string.Join(delimiter, t));
            }
        }


        /*
        private void configureReader()
        {
            reader.Name = "Reader Impinj N#-"+myReader.readerId;
            reader.ConnectTimeout = myReader.connectTimeout;
            this.settingsMode();
        }

        private void settingsMode()
        {
            settings.AutoStart.Mode = AutoStartMode.Immediate;

            settings.AutoStop.Mode = AutoStopMode.None;

            settings.Gpos.GetGpo(1).Mode = GpoMode.LLRPConnectionStatus;

            settings.Report.IncludeAntennaPortNumber = true;

            settings.ReaderMode = ReaderMode.AutoSetDenseReader;
            
            settings.SearchMode = SearchMode.DualTarget;
            
            settings.Session = myReader.session;

            try
            {
                settings.Antennas.EnableAll();
            }
            catch (Exception e)
            {
                throw e;
            }

            // Tell the reader to include the timestamp in all tag reports.
            settings.Report.IncludeFirstSeenTime = true;
            settings.Report.IncludeLastSeenTime = true;
            settings.Report.IncludeSeenCount = true;
            settings.Report.IncludePeakRssi = true;

            // If this application disconnects from the 
            // reader, hold all tag reports and events.
            settings.HoldReportsOnDisconnect = true;

            // Enable keepalives.
            settings.Keepalives.Enabled = true;
            settings.Keepalives.PeriodInMs = 5000;

            // Enable link monitor mode.
            // If our application fails to reply to
            // five consecutive keepalive messages,
            // the reader will close the network connection.
            settings.Keepalives.EnableLinkMonitorMode = true;
            settings.Keepalives.LinkDownThreshold = 5;
        }

        private void SetAntenna(Models.Antenna antenna)
        {
       
            for(int i = 0; i<=myReader.antennas; i++)
            {
                ushort num = 1;
                settings.Antennas.GetAntenna(num).TxPowerInDbm = antenna.power;
                settings.Antennas.GetAntenna(num).RxSensitivityInDbm = antenna.sensibility;
                num++;
            }
        }

        static void OnConnectionLost(ImpinjReader reader)
        {
            // This event handler is called if the reader  
            // stops sending keepalive messages (connection lost).
            Console.WriteLine("Connection lost : {0} ({1})", reader.Name, reader.Address);

            // Cleanup
            reader.Disconnect();

            // Try reconnecting to the reader
            ConnectToReader();
        }

        static void OnKeepaliveReceived(ImpinjReader reader)
        {
            // This event handler is called when a keepalive 
            // message is received from the reader.
            Console.WriteLine("Keepalive received from {0} ({1})", reader.Name, reader.Address);
        }

        static void OnTagsReported(ImpinjReader sender, TagReport report)
        {
            // This event handler is called asynchronously 
            // when tag reports are available.
            // Loop through each tag in the report 
            // and print the data.
            foreach (Tag tag in report)
            {
                string[] dataset = { tag.Epc + delimiter + tag.PeakRssiInDbm + delimiter + tag.LastSeenTime + delimiter + tag.AntennaPortNumber };
                data.Add(dataset);
                Console.WriteLine("EPC : {0}  Antenna : {1} Timestamp : {2} RSSI {3}", tag.Epc, tag.AntennaPortNumber, tag.LastSeenTime, tag.PeakRssiInDbm);
            }
        }

        private void connectReader()
        {
            try
            {
                //reader.MaxConnectionAttempts = 15;
                reader.Connect(myReader.ipAdress);
                reader.ResumeEventsAndReports();
            }
            catch (OctaneSdkException e)
            {
                throw e;
            }
        }


        public Boolean checkConnexion()
        {
            return reader.IsConnected;
        }

        public void startScan()
        {
            reader.Start();
        }

        public void stopScan()
        {
            reader.Stop();
        }

        private void saveFile()
        {

        }*/

    }
}