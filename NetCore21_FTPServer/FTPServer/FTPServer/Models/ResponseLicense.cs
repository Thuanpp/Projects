using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTPServer.Models
{
    public class ResponseLicense
    {
        public ResponseStatus Status { get; set; }
        public PmlicenceKeyHis LicenceKeyHis { get; set; }
    }
}
