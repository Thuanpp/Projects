using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PMLicenseManagement;

namespace MocauManagement
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void btnLicenseList_Click(object sender, EventArgs e)
        {
            F_LICENSE_LIST frm = new F_LICENSE_LIST();
            frm.ShowDialog();
        }

        private void btnUploadVersion_Click(object sender, EventArgs e)
        {
            F_UPLOAD_VERSION frm = new F_UPLOAD_VERSION();
            frm.ShowDialog();
        }
    }
}
