using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService_RFIDReader.Models
{
    public class Reader
    {
        public Reader()
        {

        }

        public Reader(string ip) 
        {
            this.ipAdress = ip;
            this.maxConnectionAttempts = 15;
            this.connectTimeout = 6000;
        }

        public Reader(int id,string ip, int maxAttempts, int timeout)
        {
            this.readerId = id;
            this.ipAdress = ip;
            this.maxConnectionAttempts = maxAttempts;
            this.connectTimeout = timeout;
            this.session = 1;
        }

        public int readerId { set; get; }
        public string ipAdress { set; get; }
        public int maxConnectionAttempts { set; get; }
        public int connectTimeout { set; get; }
        public ushort antennas { set; get; }
        public ushort session { set; get; }

    }
}