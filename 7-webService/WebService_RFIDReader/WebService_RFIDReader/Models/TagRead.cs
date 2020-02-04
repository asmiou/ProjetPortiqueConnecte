using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebService_RFIDReader.Models
{
    public class TagRead{

        public TagRead(string t, string e, double r, int a){
            ecp = e;
            timesTamp = t;
            rssi = r;
            antenna = a;
        }
        public string ecp { set; get; }
        public string timesTamp { set; get; }
        public double rssi { set; get; }
        public int antenna { set; get; }
    }
}