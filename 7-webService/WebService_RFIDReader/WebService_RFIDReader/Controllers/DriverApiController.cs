using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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

        public DriverApiController()
        {
            setting = new Services.LoadSettings();
            myReader = setting.reader;
            antenna = setting.antenna;
            readerService = new Services.ImpinjReaderService(myReader, antenna);
            httpClient = new HttpClient();
        }

        [HttpGet]
        [Route("start")]
        public HttpResponseMessage startScan()
        {
            try
            {
                readerService.ConnectToReader();
                readerService.startScan();
                string msg = "Code: 200, Message: Lecture en coure ...";
                return this.Request.CreateResponse(HttpStatusCode.OK,msg);
            }
            catch (Exception e)
            {
                string msg = "Code: 500, Message: Erreur de lecture vérifier la connectivité du lecteur";
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError,msg);
                //throw e;
            }
        }

        [HttpGet]
        [Route("stop")]
        public async Task<HttpResponseMessage> stopScan()
        {
            string token;
            try
            {
                token = DateTime.Now.ToString("yyyyMMddHHmmss");
                readerService.stopScan(token);
                Debug.WriteLine("Stoped...  ");
                //string uri = "http://127.0.0.1:5000/flaskapp/predict?token=" + "20200310142930";
                string uri = "http://127.0.0.1:5000/flaskapp/predict?token=" + token;
                Debug.WriteLine("url : " + uri);
                string response = await httpClient.GetStringAsync(uri);
                Debug.WriteLine("Response : "+response);
                return this.Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception e)
            {
                string msg = "Code: 500, Message: Erreur vérifier que le serveur python est lancé";
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, msg);
            }
            
        }

        [HttpGet]
        [Route("download/{token}")]
        public HttpResponseMessage downloadFile(string token){
            string path = HttpContext.Current.Server.MapPath("~/Assets/3-PredictedData/") +token+".csv";
            
            if (!File.Exists(path)) {
                string msg = "Code: 500, Message: Le fichier n'existe pas.";
                return this.Request.CreateResponse(HttpStatusCode.NotFound, msg);
            }

            try
            {
                var dataBytes = File.ReadAllBytes(path);
                var dataStream = new MemoryStream(dataBytes);

                HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
                httpResponseMessage.Content = new StreamContent(dataStream);
                httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                httpResponseMessage.Content.Headers.ContentDisposition.FileName = "data_" + token + ".csv";
                httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/csv");
                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                string msg = "Code: 500, Message: Erreur lors du chargement du fichier";
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, msg);
            }
            
        }
    }
}
