using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FTPServer.Models
{
    public class MyFileInfo
    {
        public int CurrentFileSize { get; set; }
        public string FileContent { get; set; }
        public int FileCount { get; set; }
        public string FileName { get; set; }
        public string FileOperation { get; set; }
        public int FileSequence { get; set; }
        public bool MultiplePart { get; set; }
        public long TotalFileSize { get; set; }
    }
}