using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebService_RFIDReader.Services
{
    public class LoadSettings
    {
        public Models.Reader reader { get; set; }
        public Models.Antenna antenna { get; set; }

        public LoadSettings()
        {
            loadJson();
        }
        public void loadJson()
        {
            using (StreamReader r = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/Assets/Settings.json")))
            {
                string json = r.ReadToEnd();
                Settings settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(json);

                reader = new Models.Reader(settings.readerId, settings.ipAdress, settings.maxConnectionAttempts, settings.connectTimeout);
                antenna = new Models.Antenna(settings.power, settings.sensibility);
            }
        }

        public class Settings
        {
            public int readerId;
            public string ipAdress;
            public int maxConnectionAttempts;
            public int connectTimeout;
            public ushort nbAntenna;
            public ushort session;

            public int power;
            public int sensibility;
        }
    }
}