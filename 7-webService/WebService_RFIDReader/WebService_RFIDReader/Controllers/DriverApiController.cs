using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebService_RFIDReader.Controllers
{
    [RoutePrefix("api/reader")]
    public class DriverApiController : ApiController
    {
        Services.LoadSettings setting;
        Models.Reader myReader;
        Models.Antenna antenna;

        public DriverApiController()
        {
            setting = new Services.LoadSettings();
            myReader = setting.reader;
            antenna = setting.antenna;

        }

        [HttpGet]
        [Route("start")]
        public string start()
        {
            string s = myReader.ipAdress;
            return s;
        }

        [HttpGet]
        [Route("stop")]
        public Boolean stop()
        {
            return false;
        }
    }
}
