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
        Services.ImpinjReaderService readerService;

        public DriverApiController()
        {
            setting = new Services.LoadSettings();
            myReader = setting.reader;
            antenna = setting.antenna;
            readerService = new Services.ImpinjReaderService(myReader, antenna);

        }

        [HttpGet]
        [Route("start")]
        public void start()
        {
            try
            {
                readerService.startScan();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            
        }

        [HttpGet]
        [Route("stop")]
        public void stop()
        {
            try
            {
                readerService.stopScan();
            }catch(Exception e)
            {
                throw e;
            }
            
            
        }
    }
}
