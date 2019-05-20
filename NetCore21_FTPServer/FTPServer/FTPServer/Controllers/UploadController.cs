using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FTPServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(string Folder)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json");
                var configuration = builder.Build();
                string RootPath = configuration["RootPath"];

                string fullPath = Path.Combine(RootPath, Folder);
                if (Directory.Exists(fullPath))
                    Directory.Delete(fullPath, true);

                Directory.CreateDirectory(fullPath);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }


        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json");
                var configuration = builder.Build();
                string RootPath = configuration["RootPath"];

                using (var ms = new MemoryStream())
                {
                    Request.Body.CopyTo(ms);

                    byte[] receive = ms.ToArray();

                    /* lấy 4 bytes dầu tiên là kích thước của file path */
                    int FolderNameLen = BitConverter.ToInt32(receive, 0);

                    /* lấy file path bắt đầu từ vị trí thứ 4 */
                    string sRelativePath = Encoding.UTF8.GetString(receive, 4, FolderNameLen);

                    string sFullFilePath = Path.Combine(RootPath, sRelativePath);

                    byte[] buffer = receive.Skip(4 + FolderNameLen).ToArray();

                    if (System.IO.File.Exists(sFullFilePath))
                    {
                        using (var file = System.IO.File.Open(sFullFilePath, FileMode.Append, FileAccess.Write))
                        {
                            using (var writer = new StreamWriter(file)) // buffer size can be adjusted if necessary
                            {
                                writer.BaseStream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    else
                    {
                        string parentFolder = System.IO.Directory.GetParent(sFullFilePath).FullName;

                        if (!Directory.Exists(parentFolder))
                            Directory.CreateDirectory(parentFolder);

                        using (var file = System.IO.File.Open(sFullFilePath, FileMode.CreateNew, FileAccess.Write))
                        {
                            using (var writer = new StreamWriter(file)) // buffer size can be adjusted if necessary
                            {
                                writer.BaseStream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }

                    return Ok();
                }
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}