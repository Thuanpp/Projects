using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FTPServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [Route("api/Upload/RawBinary")]
        public IActionResult RawBinary()
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    Request.Body.CopyToAsync(ms);

                    byte[] receive = ms.ToArray();

                    /* lấy 4 bytes dầu tiên là kích thước của folder path */
                    int FolderNameLen = BitConverter.ToInt32(receive, 0);

                    /* lấy folder path bắt đầu từ vị trí thứ 4 */
                    string sRelativePath = Encoding.UTF8.GetString(receive, 4, FolderNameLen);

                    string sFullFolderPath = System.IO.Path.Combine("", sRelativePath);

                    return Ok();
                    //return ms.ToArray();  // returns base64 encoded string JSON result
                }
            }
            catch
            {
                return BadRequest();
            }

            
            //string fileName = myFileInfo.FileName;
            //string fullPath = Path.Combine(m_SaveFilePath, fileName);

            //if (File.Exists(fullPath))
            //{
            //    using (var file = File.Open(fullPath, FileMode.Append, FileAccess.Write))
            //    {
            //        using (var writer = new StreamWriter(file)) // buffer size can be adjusted if necessary
            //        {
            //            writer.BaseStream.Write(buffer, 0, buffer.Length);
            //        }
            //    }
            //}
            //else
            //{
            //    string parentFolder = System.IO.Directory.GetParent(fullPath).FullName;

            //    if (!Directory.Exists(parentFolder))
            //        Directory.CreateDirectory(parentFolder);

            //    using (var file = File.Open(fullPath, FileMode.CreateNew, FileAccess.Write))
            //    {
            //        using (var writer = new StreamWriter(file)) // buffer size can be adjusted if necessary
            //        {
            //            writer.BaseStream.Write(buffer, 0, buffer.Length);
            //        }
            //    }
            //}


        }
    }
}