using FTPServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Http;

namespace FTPServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FilesController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        //string RootPath = "";

        [HttpGet]
        public ActionResult<FileListInfo> Get(string folderPath)
        {
            //var builder = new ConfigurationBuilder()
            //                      .SetBasePath(Directory.GetCurrentDirectory())
            //                      .AddJsonFile("appsettings.json");
            //var configuration = builder.Build();
            //RootPath = configuration["RootPath"];

            FileListInfo info = new FileListInfo();
            ResponseStatus res = new ResponseStatus();
            string fullPath = Path.Combine(_hostingEnvironment.ContentRootPath,folderPath);
            if (!Directory.Exists(fullPath))
            {
                res.StatusCode = HttpStatusCode.BadRequest.ToString();
                res.Message = "Folder is not exist";
            }
            try
            {
                string[] fileList = Directory.GetFiles(fullPath, "*.*", SearchOption.TopDirectoryOnly);
                res.StatusCode = HttpStatusCode.OK.ToString();
                res.Message = "Successful";

                info.Status = res;
                List<MyFileInfo> list = new List<MyFileInfo>();
                for (int i = 0; i < fileList.Length; i++)
                {

                    MyFileInfo file = new MyFileInfo();
                    file.FileName = fileList[i].Substring(_hostingEnvironment.ContentRootPath.Length);
                    file.TotalFileSize = new FileInfo(fileList[i]).Length;
                    list.Add(file);
                }
                info.MyFileInfoList = list;
            }
            catch(Exception ex)
            {
                res.StatusCode = HttpStatusCode.BadRequest.ToString();
                res.Message = ex.Message.ToString();

                info.Status = res;
            }

            return info;

        }

    }
}
