using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Configuration;

namespace FTPServer.Controllers
{
    public class FolderController : ApiController
    {
        string RootPath = ConfigurationManager.AppSettings["RootPath"];
        public IEnumerable<string> Get()
        {

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
