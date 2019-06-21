using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MocauManagementDAL;
using Utility;

namespace PMLicenseManagement
{
    public partial class F_LICENSE_LIST : Form
    {
        public F_LICENSE_LIST()
        {
            InitializeComponent();
        }

        private enum ColumnOrder
        {
            colCustomer,
            colPhone,
            colEmail,
            colKey,
            colActiveLimit,
            colDayOfUse,
            colMaxVersion,
            colDetail
        }

        private void FormatGridview()
        {
            dgvLicenseList.AutoGenerateColumns = false;
            dgvLicenseList.Columns["colCustomer"].DisplayIndex = (int)ColumnOrder.colCustomer;
            dgvLicenseList.Columns["colPhone"].DisplayIndex = (int)ColumnOrder.colPhone;
            dgvLicenseList.Columns["colEmail"].DisplayIndex = (int)ColumnOrder.colEmail;
            dgvLicenseList.Columns["colKey"].DisplayIndex = (int)ColumnOrder.colKey;
            dgvLicenseList.Columns["colActiveLimit"].DisplayIndex = (int)ColumnOrder.colActiveLimit;
            dgvLicenseList.Columns["colDayOfUse"].DisplayIndex = (int)ColumnOrder.colDayOfUse;
            dgvLicenseList.Columns["colMaxVersion"].DisplayIndex = (int)ColumnOrder.colMaxVersion;
            dgvLicenseList.Columns["colDetail"].DisplayIndex = (int)ColumnOrder.colDetail;
        }

        private void LoadDataToGridview()
        {
            using (var db = new PMLicenceDevEntities())
            {
                var listKey = db.PMLicenceKeys.Select(x => x).ToList();
                if (listKey != null)
                    dgvLicenseList.DataSource = listKey;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FormatGridview();
            LoadDataToGridview();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            F_LICENSE_UPDATE frmAdd = new F_LICENSE_UPDATE();
            frmAdd.ShowDialog();
            if (frmAdd.Result == (int)DialogResult.Cancel) return;
            string publicKey = frmAdd.LicenceKey.PublicKey;
            frmAdd.Dispose();
            frmAdd = null;
            LoadDataToGridview();
            foreach (DataGridViewRow r in dgvLicenseList.Rows)
            {
                if (r.Cells["colKey"].Value.ToString() == publicKey)
                {
                    dgvLicenseList.CurrentCell = dgvLicenseList.Rows[r.Index].Cells[0];
                    dgvLicenseList.Rows[r.Index].Selected = true;
                    break;
                }
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                int rowSelected = dgvLicenseList.CurrentRow.Index;
                PMLicenceKey _PMLicenseKey = new PMLicenceKey();

                _PMLicenseKey.CusName = dgvLicenseList.Rows[rowSelected].Cells["colCustomer"].Value.ToString();
                _PMLicenseKey.CusPhone = dgvLicenseList.Rows[rowSelected].Cells["colPhone"].Value.ToString();
                _PMLicenseKey.CusEmail = dgvLicenseList.Rows[rowSelected].Cells["colEmail"].Value.ToString();
                _PMLicenseKey.PublicKey = dgvLicenseList.Rows[rowSelected].Cells["colKey"].Value.ToString();
                _PMLicenseKey.LimitActived = int.Parse(dgvLicenseList.Rows[rowSelected].Cells["colActiveLimit"].Value.ToString());
                _PMLicenseKey.DayOfUse = int.Parse(dgvLicenseList.Rows[rowSelected].Cells["colDayOfUse"].Value.ToString());

                F_LICENSE_UPDATE frmEdit = new F_LICENSE_UPDATE();
                frmEdit.Mode = (int)Global.DataMode.Update;
                frmEdit.LicenceKey = _PMLicenseKey;
                frmEdit.ShowDialog();
                if (frmEdit.Result == (int)DialogResult.Cancel) return;
                frmEdit.Dispose();
                frmEdit = null;
                LoadDataToGridview();
                dgvLicenseList.CurrentCell = dgvLicenseList.Rows[rowSelected].Cells[0];
                dgvLicenseList.Rows[rowSelected].Selected = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you want to delete?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                int rowSelected = dgvLicenseList.CurrentRow.Index;
                string publicKey = dgvLicenseList.Rows[rowSelected].Cells["colKey"].Value.ToString();
                using (var db = new PMLicenceDevEntities())
                {
                    var licenkey = db.PMLicenceKeys.FirstOrDefault(x => x.PublicKey == publicKey);
                    if (licenkey != null)
                    {
                        db.PMLicenceKeys.Remove(licenkey);
                        db.SaveChanges();
                        LoadDataToGridview();
                    }
                }
            }
           
        }

        private void dgvLicenseList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != (int)ColumnOrder.colDetail) return;

            int rowSelected = dgvLicenseList.CurrentRow.Index;
            string publicKey = dgvLicenseList.Rows[rowSelected].Cells["colKey"].Value.ToString();
            F_LICENSE_KEY_DETAIL frmDetail = new F_LICENSE_KEY_DETAIL();
            frmDetail.PublicKey = publicKey;
            frmDetail.ShowDialog();
            frmDetail.Dispose();
            frmDetail = null;
        }
    }
}
