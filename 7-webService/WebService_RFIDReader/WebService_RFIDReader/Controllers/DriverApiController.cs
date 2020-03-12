using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
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
        HttpClient httpClient;
        string token;


        public DriverApiController()
        {
            setting = new Services.LoadSettings();
            myReader = setting.reader;
            antenna = setting.antenna;
            readerService = new Services.ImpinjReaderService(myReader, antenna);
            httpClient = new HttpClient();
            token = readerService.today;

        }

        [HttpGet]
        [Route("start")]
        public void startScan()
        {
            try
            {
                readerService.ConnectToReader();
                readerService.startScan();
                Debug.WriteLine("Started...");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        [Route("stop")]
        public async Task<string> StopScan()
        {
            try
            {
                Debug.WriteLine("begin...  ");
                //string uri = "http://127.0.0.1:5000/flaskapp/predict?token=" + "20200310142930";
                string uri = "http://127.0.0.1:5000/flaskapp/predict?token=" + token;
                Debug.WriteLine("url : " + uri);
                string response = await httpClient.GetStringAsync(uri);
                Debug.WriteLine("Response : "+response);
                return response;
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
    }
}
