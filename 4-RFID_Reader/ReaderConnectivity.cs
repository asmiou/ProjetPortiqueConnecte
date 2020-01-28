using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Impinj.OctaneSdk;

namespace OctaneSdkUseCases
{
    class Program
    {
        const string READER_HOSTNAME = "192.168.0.20"; 
        // Create an instance of the ImpinjReader class.
        static ImpinjReader reader = new ImpinjReader();
        static  string filename = "FirstTest";
        static string delimiter = ",";
        static string today = DateTime.Now.ToString().Replace('/', '_').Replace(':', '-').Replace(' ','H');
        static string filepath = "data/"+filename+"-"+today+".csv";
        public static List<string[]> data=new List<string[]>();
        static void ConnectToReader()
        {
            try
            {
                Console.WriteLine("Attempting to connect to {0} ({1}).",
                    reader.Name, READER_HOSTNAME); 
                
                // The maximum number of connection attempts
                // before throwing an exception.
                reader.MaxConnectionAttempts = 15;
                // Number of milliseconds before a 
                // connection attempt times out.
                reader.ConnectTimeout = 6000;
                // Connect to the reader.
                // Change the ReaderHostname constant in SolutionConstants.cs 
                // to the IP address or hostname of your reader.
                reader.Connect(READER_HOSTNAME);
                Console.WriteLine("Successfully connected.");

                // Tell the reader to send us any tag reports and 
                // events we missed while we were disconnected.
                reader.ResumeEventsAndReports();
            }
            catch (OctaneSdkException e)
            {
                Console.WriteLine("Failed to connect."); 
                throw e;
            }
        }
        
        static void Main(string[] args)
        {
            try
            {
                // Assign a name to the reader. 
                // This will be used in tag reports. 
                reader.Name = "Reader Impinj";

                // Connect to the reader.
                ConnectToReader();

                // Get the default settings.
                // We'll use these as a starting point
                // and then modify the settings we're 
                // interested in.
                Settings settings = reader.QueryDefaultSettings();

                // Start the reader as soon as it's configured.
                // This will allow it to run without a client connected.
                settings.AutoStart.Mode = AutoStartMode.Immediate;
                settings.AutoStop.Mode = AutoStopMode.None;

                // Use Advanced GPO to set GPO #1 
                // when an client (LLRP) connection is present.
                settings.Gpos.GetGpo(1).Mode = GpoMode.LLRPConnectionStatus;
                /*
                 * START WHAT WE ADDED
                 * 
                 */

                settings.Report.IncludeAntennaPortNumber = true;

                // The reader can be set into various modes in which reader
                // dynamics are optimized for specific regions and environments.
                // The following mode, AutoSetDenseReader, monitors RF noise and interference and then automatically
                // and continuously optimizes the reader’s configuration
                settings.ReaderMode = ReaderMode.AutoSetDenseReader;
                settings.SearchMode = SearchMode.DualTarget;
                //settings.SearchMode = SearchMode.SingleTarget;
                settings.Session = 1;

                // Enable antenna #1. Disable all others. You may want to use more antennas, for my test case I
                // only was using one.
                //settings.Antennas.DisableAll();
                //settings.Antennas.GetAntenna(1).IsEnabled = true;

                try {
                    settings.Antennas.EnableAll();
                    //to enable only one Antenna 
                   // settings.Antennas.DisableAll();
                    //settings.Antennas.GetAntenna(1).IsEnabled = true;
                    
                }
                catch (Exception e) {
                    Console.WriteLine("Erreur in Enable All: " + e.ToString());
                }

                // Set the Transmit Power and
                // Receive Sensitivity on the antennas.
                //Antenna 1
                settings.Antennas.GetAntenna(1).TxPowerInDbm = 27;
                settings.Antennas.GetAntenna(1).RxSensitivityInDbm = -70;
                //Antenna 2
                settings.Antennas.GetAntenna(2).TxPowerInDbm = 27;
                settings.Antennas.GetAntenna(2).RxSensitivityInDbm = -70;
                //Antenna 3
                settings.Antennas.GetAntenna(3).TxPowerInDbm = 27;
                settings.Antennas.GetAntenna(3).RxSensitivityInDbm = -70;
                //Antenna 4
                settings.Antennas.GetAntenna(4).TxPowerInDbm = 27;
                settings.Antennas.GetAntenna(4).RxSensitivityInDbm = -70;
                /*
                 * END WHAT WE ADDED
                 * 
                 */
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

                // Assign an event handler that will be called
                // when keepalive messages are received.
                reader.KeepaliveReceived += OnKeepaliveReceived;

                // Assign an event handler that will be called
                // if the reader stops sending keepalives.
                reader.ConnectionLost += OnConnectionLost;

                // Apply the newly modified settings.
                reader.ApplySettings(settings);

                // Save the settings to the reader's 
                // non-volatile memory. This will
                // allow the settings to persist
                // through a power cycle.
                reader.SaveSettings();

                // Assign the TagsReported event handler.
                // This specifies which method to call
                // when tags reports are available.
                reader.TagsReported += OnTagsReported;

                // Wait for the user to press enter.
                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();

                // Stop reading.
                reader.Stop();
                var file = File.CreateText(filepath);
                foreach(var a in data)
                {
                    file.WriteLine(string.Join(delimiter, a));
                }
                // Disconnect from the reader.
                reader.Disconnect();
            }
            catch (OctaneSdkException e)
            {
                // Handle Octane SDK errors.
                Console.WriteLine("Octane SDK exception: {0}", e.Message);
            }
            catch (Exception e)
            {
                // Handle other .NET errors.
                Console.WriteLine("Exception : {0}", e.Message);
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
               string[] dataset= { tag.Epc + delimiter + tag.PeakRssiInDbm + delimiter + tag.LastSeenTime + delimiter + tag.AntennaPortNumber };
                data.Add(dataset);
             Console.WriteLine("EPC : {0}  Antenna : {1} Timestamp : {2} RSSI {3}", tag.Epc,tag.AntennaPortNumber, tag.LastSeenTime,tag.PeakRssiInDbm);
            }
        }
    }
}
