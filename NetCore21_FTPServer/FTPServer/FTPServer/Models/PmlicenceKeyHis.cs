using System;
using System.Collections.Generic;

namespace FTPServer.Models
{
    public partial class PmlicenceKeyHis
    {
        public string Hwkey { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public DateTime ActivedDate { get; set; }
        public DateTime CurrentDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string SoftwareVersion { get; set; }

        public PmlicenceKey PublicKeyNavigation { get; set; }
    }
}
