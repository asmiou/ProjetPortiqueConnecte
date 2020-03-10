using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        Services.RunPythonScript pythonService;


        public DriverApiController()
        {
            //setting = new Services.LoadSettings();
            //myReader = setting.reader;
            //antenna = setting.antenna;
            //readerService = new Services.ImpinjReaderService(myReader, antenna);
            pythonService = new Services.RunPythonScript();


        }

        [HttpGet]
        [Route("start")]
        public void start()
        {
            try
            {
                //readerService.ConnectToReader();
                //readerService.startScan();
                System.Diagnostics.Debug.WriteLine("Started...");
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
                //readerService.stopScan();
                //string rawData = readerService.prefixPath + "_scan.csv";
                //buildDataSet(rawData);
                //string train = readerService.prefixPath + "_trainSet.csv";
                //string test = readerService.prefixPath + "_testSet.csv";
                //string dataSet = readerService.prefixPath + "_dataSet.csv";
                //knnPredict(train, test, dataSet);

                //pythonService.hello();
                string path = HostingEnvironment.MapPath(@"~/Assets");

                
                if (pythonService.buildDataSet(path, "20200310142930"))
                {
                    Console.WriteLine("executed...");
                }
                else
                {
                    Console.WriteLine("no executed...");
                }
                Console.WriteLine("stopped...");

            }
            catch (Exception e)
            {
                throw e;
            }
            
        }

        private void buildDataSet(string rawDataPath)
        {
            string loc = HttpContext.Current.Server.MapPath("~/App_Data/11-RawData/");
            try
            {
                //pythonService.buildDataSet(loc+rawDataPath);
            }catch(Exception e)
            {
                throw e;
            }
            
        }

        private void knnPredict(string trainSet, string testSet, string dataSet)
        {
            try
            {
                string loc = HttpContext.Current.Server.MapPath("~/App_Data/2-TransformedData/");
                pythonService.predictByKnn(loc+trainSet, loc + testSet, loc + dataSet);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private void UploadData()
        {

        }
    }
}
