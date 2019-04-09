using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTPServer.Models
{
    public class FolderListInfo
    {
        public ResponseStatus Status { get; set; }
        public List<string> PathList { get; set; }
    }
}