using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTPServer.Models
{
    public class FileListInfo
    {
        public ResponseStatus Status { get; set; }
        public List<MyFileInfo> MyFileInfoList { get; set; }
    }
}