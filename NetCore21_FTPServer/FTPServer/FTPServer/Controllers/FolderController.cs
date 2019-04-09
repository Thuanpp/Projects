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

namespace FTPServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        string RootPath = "";

        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var builder = new ConfigurationBuilder()
                                   .SetBasePath(Directory.GetCurrentDirectory())
                                   .AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            RootPath = configuration["RootPath"];
            List<string> pathList = new List<string>();
            pathList.Add(""); // Path for RootPath
            GetAllSubFolder(RootPath, pathList);
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
