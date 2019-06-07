using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using FTPServer.Models;

namespace FTPServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public FolderController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        string RootPath = "";

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get(string rootPath, string maxVersion)
        {
            RootPath = Path.Combine(_hostingEnvironment.ContentRootPath, rootPath, maxVersion);

            List<string> pathList = new List<string>();
            pathList.Add(""); // Path for RootPath
            GetAllSubFolder(RootPath, pathList);
            for (int i = 0; i < pathList.Count; i++)
                pathList[i] = Path.Combine(rootPath, maxVersion) + pathList[i];
            return pathList;
        }

        private void GetAllSubFolder(string rootPath, List<string> pathList)
        {
            string[] subDirs;
            try
            {
                subDirs = Directory.GetDirectories(rootPath);
            }
            catch(UnauthorizedAccessException e)
            {
                return;
            }
            catch (DirectoryNotFoundException e)
            {
                return;
            }
            catch (Exception e)
            {
                return;
            }
            foreach (string subDir in subDirs)
            {
                GetSubFolders(subDir, pathList);
            }
        }

        private void GetSubFolders(string path, List<string> pathList)
        {
            string[] subDirs;
            try
            {
                subDirs = Directory.GetDirectories(path);
            }
            catch (UnauthorizedAccessException e)
            {
                return;
            }
            catch (DirectoryNotFoundException e)
            {
                return;
            }
            catch(Exception e)
            {
                return;
            }

            pathList.Add(path.Substring(RootPath.Length));
            foreach (string subDir in subDirs)
            {
                GetSubFolders(subDir, pathList);
            }
        }
    }
}
