using FTPServer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace FTPServer.Controllers
{
    public class FileContentController : ApiController
    {
        string RootPath = ConfigurationManager.AppSettings["RootPath"];
        long restFileChunkSize = Utility.ConvertMegaBytesToBytes(
                    double.Parse(ConfigurationManager.AppSettings["restFileChunkSize"]));
        public HttpResponseMessage Get(string filePath)
        {

            string XML = "<note><body>Message content</body></note>";
            return new HttpResponseMessage()
            {
                Content = new StringContent(XML, Encoding.UTF8, "application/xml")
            };
        }

        // POST api/values
        public HttpResponseMessage Post([FromBody]MyFileInfo value)
        {

            //FileContentInfo fileContentInfo = new FileContentInfo();
            HttpResponseMessage httpResponseMessage = null;
            ResponseStatus res = new ResponseStatus();
            string fullPath = RootPath + value.FileName;
            byte[] buffer;
            MemoryStream dataStream;
            try
            {
                if (value.MultiplePart == false) 
                {

                    byte[] dataBytes = File.ReadAllBytes(fullPath);
                    //add bytes to memory stream 
                    dataStream = new MemoryStream(dataBytes);

                    httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
                    httpResponseMessage.Content = new StreamContent(dataStream);
                    httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    httpResponseMessage.Content.Headers.ContentDisposition.FileName = value.FileName;
                    httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                }
                else // Trường hợp không chia file thành nhiều phần 
                {
                    buffer = new byte[value.CurrentFileSize];
                    using (var file = File.OpenRead(fullPath))
                    {
                        file.Position = (value.FileSequence - 1) * (int)restFileChunkSize;
                        file.Read(buffer, 0, value.CurrentFileSize);

                        dataStream = new MemoryStream(buffer);

                        httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
                        httpResponseMessage.Content = new StreamContent(dataStream);
                        httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                        httpResponseMessage.Content.Headers.ContentDisposition.FileName = value.FileName;
                        httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    }

                }
            }
            catch (Exception ex)
            {
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return httpResponseMessage;
        }
    }
}
