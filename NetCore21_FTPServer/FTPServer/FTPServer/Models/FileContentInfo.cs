using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTPServer.Models
{
    public class FileContentInfo
    {
        public ResponseStatus Status { get; set; }
        public MyFileInfo MyFileInfo { get; set; }
    }
}