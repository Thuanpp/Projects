using FTPServer.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Http;

namespace FTPServer.Controllers
{
    public class FilesController : ApiController
    {
        string RootPath = ConfigurationManager.AppSettings["RootPath"];
        public FileListInfo Get(string folderPath)
        {
            FileListInfo info = new FileListInfo();
            ResponseStatus res = new ResponseStatus();
            string fullPath = RootPath + folderPath;
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
                    file.FileName = fileList[i].Substring(RootPath.Length);
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
