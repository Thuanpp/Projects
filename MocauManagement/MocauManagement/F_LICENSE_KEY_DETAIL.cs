using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MocauManagementDAL;

namespace PMLicenseManagement
{
    public partial class F_LICENSE_KEY_DETAIL : Form
    {

        public string PublicKey { get; set; }

        public F_LICENSE_KEY_DETAIL()
        {
            InitializeComponent();
        }

        private enum ColumnOrder
        {
            colHWKey,
            colActivedDate,
            colCurrentDate,
            colExpiredDate,
            colSoftwareVersion
        }

        private void FormatGridview()
        {
            dgvLicenseList.AutoGenerateColumns = false;
            dgvLicenseList.Columns["colHWKey"].DisplayIndex = (int)ColumnOrder.colHWKey;
            dgvLicenseList.Columns["colActivedDate"].DisplayIndex = (int)ColumnOrder.colActivedDate;
            dgvLicenseList.Columns["colCurrentDate"].DisplayIndex = (int)ColumnOrder.colCurrentDate;
            dgvLicenseList.Columns["colExpiredDate"].DisplayIndex = (int)ColumnOrder.colExpiredDate;
            dgvLicenseList.Columns["colSoftwareVersion"].DisplayIndex = (int)ColumnOrder.colSoftwareVersion;
        }

        private void LoadDataToGridview()
        {
            using (var db = new PMLicenceDevEntities())
            {
                var list = db.PMLicenceKeyHis.Where(x => x.PublicKey == PublicKey).ToList();
                if (list != null)
                    dgvLicenseList.DataSource = list;
            }
        }

        private void F_LICENSE_KEY_DETAIL_Load(object sender, EventArgs e)
        {
            FormatGridview();
            LoadDataToGridview();
        }
    }
}
