using System;
using System.Collections.Generic;

namespace FTPServer.Models
{
    public partial class PmlicenceKey
    {
        public PmlicenceKey()
        {
            PmlicenceKeyHis = new HashSet<PmlicenceKeyHis>();
        }

        public string PublicKey { get; set; }
        public int LimitActived { get; set; }
        public string CusName { get; set; }
        public string CusPhone { get; set; }
        public string CusEmail { get; set; }
        public int DayOfUse { get; set; }

        public ICollection<PmlicenceKeyHis> PmlicenceKeyHis { get; set; }
    }
}
