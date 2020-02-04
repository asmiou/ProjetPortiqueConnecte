using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebService_RFIDReader.Models
{
    public class Antenna
    {

        public Antenna(float power, float sensibility)
        {
            this.power = power;
            this.sensibility = sensibility;
        }

        public Antenna()
        {
            this.power = 27;
            this.sensibility = -70;
        }
        public float power { set; get; }
        public float sensibility { set; get; }

    }
}