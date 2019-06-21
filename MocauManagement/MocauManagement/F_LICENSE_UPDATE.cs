using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MocauManagementDAL;
using Utility;

namespace PMLicenseManagement
{
    public partial class F_LICENSE_UPDATE : Form
    {
        public int Mode { get; set; }
        public int Result { get; set; }
        public PMLicenceKey LicenceKey { get; set; }

        public F_LICENSE_UPDATE()
        {
            InitializeComponent();
        }

        private void F_ADD_EDIT_Load(object sender, EventArgs e)
        {
            LoadDataToForm();
        }

        private void LoadDataToForm()
        {
            if (Mode == (int)Global.DataMode.Insert)
            {

            }
            else if (Mode == (int)Global.DataMode.Update)
            {
                txtCusName.Text = LicenceKey.CusName;
                txtPhone.Text = LicenceKey.CusPhone;
                txtEmail.Text = LicenceKey.CusEmail;
                txtPublicKey.Text = LicenceKey.PublicKey;
                txtActiveLimit.Text = LicenceKey.LimitActived.ToString();
                txtDayOfUse.Text = LicenceKey.DayOfUse.ToString();
                txtMaxVersion.Text = LicenceKey.MaxVersion;
                btnGenerateKey.Visible = false;
                btnValidateKey.Visible = false;
            }
        }

        private bool InputCheck()
        {

            bool isNumerical;
            int myInt;

            if (string.IsNullOrEmpty(txtPublicKey.Text.Trim()))
            {
                MessageBox.Show("PublicKey can not empty");
                return false;
            }

            if (string.IsNullOrEmpty(txtCusName.Text.Trim()))
            {
                MessageBox.Show("Customer Name can not empty");
                return false;
            }


            if (!string.IsNullOrEmpty(txtActiveLimit.Text))
            {
                isNumerical = int.TryParse(txtActiveLimit.Text, out myInt);
                if (!isNumerical)
                {
                    MessageBox.Show("Active Limit is not number");
                    return isNumerical;
                }
            }

            if (!string.IsNullOrEmpty(txtDayOfUse.Text))
            {
                isNumerical = int.TryParse(txtDayOfUse.Text, out myInt);
                if (!isNumerical)
                {
                    MessageBox.Show("Day of Use is not number");
                    return isNumerical;
                }
            }

            if (string.IsNullOrEmpty(txtMaxVersion.Text.Trim()))
            {
                MessageBox.Show("Max Version can not empty");
                return false;
            }

            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {

                if (!InputCheck()) return;
                if (Mode == (int)Global.DataMode.Insert)
                {
                    using (var db = new PMLicenceDevEntities())
                    {

                        LicenceKey = new PMLicenceKey();
                        LicenceKey.CusName = txtCusName.Text;
                        LicenceKey.CusPhone = txtPhone.Text;
                        LicenceKey.CusEmail = txtEmail.Text;
                        LicenceKey.LimitActived = string.IsNullOrEmpty(txtActiveLimit.Text) ? 0 : int.Parse(txtActiveLimit.Text);
                        LicenceKey.DayOfUse = string.IsNullOrEmpty(txtDayOfUse.Text) ? 0 : int.Parse(txtDayOfUse.Text);
                        LicenceKey.PublicKey = txtPublicKey.Text;
                        LicenceKey.RootPath = LicenceKey.PublicKey + "." + LicenceKey.CusName;
                        LicenceKey.MaxVersion = txtMaxVersion.Text;
                        db.PMLicenceKeys.Add(LicenceKey);
                        db.SaveChanges();
                    }
                }
                else if (Mode == (int)Global.DataMode.Update)
                {

                    using (var db = new PMLicenceDevEntities())
                    {
                        LicenceKey = db.PMLicenceKeys.FirstOrDefault(x => x.PublicKey == txtPublicKey.Text.Trim());
                        if (LicenceKey != null)
                        {
                            LicenceKey.CusName = txtCusName.Text;
                            LicenceKey.CusPhone = txtPhone.Text;
                            LicenceKey.CusEmail = txtEmail.Text;
                            LicenceKey.LimitActived = string.IsNullOrEmpty(txtActiveLimit.Text) ? 0 : int.Parse(txtActiveLimit.Text);
                            LicenceKey.DayOfUse = string.IsNullOrEmpty(txtDayOfUse.Text) ? 0 : int.Parse(txtDayOfUse.Text);
                            LicenceKey.MaxVersion = txtMaxVersion.Text;
                            db.SaveChanges();
                        }

                    }

                }
                Result = (int)DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Result = (int)DialogResult.Cancel;
            this.Close();
        }

        private void F_ADD_EDIT_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btnGenerateKey_Click(object sender, EventArgs e)
        {
            string key = "";
            for (int i = 0; i < 5; i++)
            {
                if (i == 4)
                    key += Utility.UtilityConvert.RandomString(5, true);
                else
                    key += Utility.UtilityConvert.RandomString(5, true) + "-";
                Thread.Sleep(100);
            }
            txtPublicKey.Text = key;
          
        }

        private void btnValidateKey_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtPublicKey.Text.Trim())) return;

            using (var db = new PMLicenceDevEntities())
            {
                var key = db.PMLicenceKeys.FirstOrDefault(x => x.PublicKey == txtPublicKey.Text.Trim());
                if (key != null)
                    MessageBox.Show("Public Key is existed. Please generate other key");
                else
                    MessageBox.Show("Public Key is valid.");
            }
        }
    }
}
