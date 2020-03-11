using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Services.RunPythonScript_Old pythonService;
        Services.RunPythonScript runPython;



        public DriverApiController()
        {
            //setting = new Services.LoadSettings();
            //myReader = setting.reader;
            //antenna = setting.antenna;
            //readerService = new Services.ImpinjReaderService(myReader, antenna);
            pythonService = new Services.RunPythonScript_Old();
            runPython = new Services.RunPythonScript();


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
                runPython.exectScript(path, "20200310142930");
                //runPython.buildDataSet(path, "20200310142930");
                //runPython.predictData(path, "20200310142930");

                /*
                if (runPython.buildDataSet(path, "20200310142930"))
                {
                    Debug.WriteLine("data builded...");

                    if (runPython.predictData(path, "20200310142930"))
                    {
                        Debug.WriteLine("data predicted...");
                    }
                    else
                    {
                        Debug.WriteLine("data not predict...");
                    }
                    
                }
                else
                {
                    Debug.WriteLine("data not build...");
                }
                */

            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
    }
}
