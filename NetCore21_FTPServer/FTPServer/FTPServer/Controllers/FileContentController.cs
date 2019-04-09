using FTPServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    [Route("api/[controller]")]
    [ApiController]
    public class FileContentController : ControllerBase
    {
        //string RootPath = ConfigurationManager.AppSettings["RootPath"];
        //long restFileChunkSize = Utility.ConvertMegaBytesToBytes(
        //            double.Parse(ConfigurationManager.AppSettings["restFileChunkSize"]));

        public HttpResponseMessage Get(string filePath)
        {

            string XML = "<note><body>Message content</body></note>";
            return new HttpResponseMessage()
            {
                Content = new StringContent(XML, Encoding.UTF8, "application/xml")
            };
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]MyFileInfo value)
        {

            long restFileChunkSize = 0;

            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            string RootPath = configuration["RootPath"];
            string strRestFileChunkSize = configuration["restFileChunkSize"];

            restFileChunkSize = Utility.ConvertMegaBytesToBytes(double.Parse(strRestFileChunkSize));
            FileContentResult ret = null;

            ResponseStatus res = new ResponseStatus();
            string fullPath = RootPath + value.FileName;
            byte[] buffer;

            try
            {
                if (value.MultiplePart == false)
                {

                    byte[] dataBytes = System.IO.File.ReadAllBytes(fullPath);
                    ret = new FileContentResult(dataBytes, "application/octet-stream");
                }
                else // Trường hợp không chia file thành nhiều phần 
                {
                    buffer = new byte[value.CurrentFileSize];
                    using (var file = System.IO.File.OpenRead(fullPath))
                    {
                        file.Position = (value.FileSequence - 1) * (int)restFileChunkSize;
                        file.Read(buffer, 0, value.CurrentFileSize);

                        ret = new FileContentResult(buffer, "application/octet-stream");
                    }

                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return ret;
        }
    }
}
