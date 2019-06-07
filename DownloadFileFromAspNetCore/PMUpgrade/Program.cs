using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace PMUpgrade
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 frm = new Form1();
            if (args != null && args.Length > 0)
            {
                //frm.PublicKey = args[0].Split('^').ElementAt(0);
                //frm.HwKey = args[0].Split('^').ElementAt(1);
            }
            Application.Run(new Form1("111-222-333.THUANPP", "1.0.0.2"));
        }
    }
}
