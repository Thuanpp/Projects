using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMUpgrade.Model
{
    public class FileListInfo
    {
        public ResponseStatus Status { get; set; }
        public List<MyFileInfo> MyFileInfoList { get; set; }
    }
}
