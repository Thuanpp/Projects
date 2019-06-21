using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MocauManagementDAL;

namespace MocauManagement
{
    public partial class F_UPLOAD_VERSION : Form
    {
        private List<PMLicenceKey> LicenceKeys = new List<PMLicenceKey>();

        public F_UPLOAD_VERSION()
        {
            InitializeComponent();
        }

        private void F_UPLOAD_VERSION_Load(object sender, EventArgs e)
        {
            LoadCustomers();
        }

       

        private void cbCustomer_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = GetSelectedItem();
            if (key != null)
                txtCurrentVersion.Text = key.MaxVersion;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateUpload())
            {
                string[] files = Directory.GetFiles(txtLocalPath.Text, "*.*", SearchOption.AllDirectories);
                if (files.Length > 0)
                {
                    var key = GetSelectedItem();
                    if (key != null)
                    {
                        string pathFolder = Path.Combine(key.RootPath, txtUploadedVersion.Text.Trim());
                        if (CreateFolderUploaded(pathFolder))
                        {
                            int ChunkSize = Properties.Settings.Default.restFileChunkSize;
                            long restFileChunkSize = Utility.UtilityConvert.ConvertMegaBytesToBytes(double.Parse(ChunkSize.ToString()));

                            progressBar1.Maximum = files.Length;
                            progressBar1.Minimum = 0;
                            progressBar1.Value = 0;
                            foreach (string f in files)
                            {
                                if (!File.Exists(f)) continue;

                                try
                                {
                                    FileInfo fInfo = new FileInfo(f);
                                    UploadFile(fInfo, restFileChunkSize, pathFolder);
                                    progressBar1.Value++;
                                }
                                catch
                                {
                                    MessageBox.Show("Error during upload");
                                    return;
                                }
                            }
                            using (var db = new PMLicenceDevEntities())
                            {
                                var LicenceKey = db.PMLicenceKeys.FirstOrDefault(x => x.PublicKey == key.PublicKey);
                                if (LicenceKey != null)
                                {
                                    LicenceKey.MaxVersion = txtUploadedVersion.Text.Trim();
                                    db.SaveChanges();
                                }
                            }
                            MessageBox.Show("uploaded successfully");
                        }
                        else
                        {
                            MessageBox.Show("Error during upload");
                            return;
                        }
                    }

                }
            }
        }

        #region Private Method
        private bool UploadFile(FileInfo f, long restFileChunkSize, string folderUploaded)
        {

            ServicePointManager.ServerCertificateValidationCallback = new
           RemoteCertificateValidationCallback
           (
               delegate { return true; }
           );

            long pos = 0;
            long remainSize = f.Length;
            string relativePath = f.FullName.Substring(txtLocalPath.Text.Trim().Length + 1);
            relativePath = Path.Combine(folderUploaded, relativePath);
            byte[] filePathByte = Encoding.UTF8.GetBytes(relativePath);
            byte[] filePathLen = BitConverter.GetBytes(filePathByte.Length);
            byte[] bytereadFile;
            byte[] buffer;
            string PostUrl = Properties.Settings.Default.Url + "/api/Upload";

            //Truong hop file co size lon hon 5MB, phai gui nhieu lan
            if (f.Length > restFileChunkSize)
            {

                while (remainSize > 0)
                {
                    using (var file = System.IO.File.OpenRead(f.FullName))
                    {
                        file.Position = pos;

                        if (remainSize > restFileChunkSize)
                        {
                            bytereadFile = new byte[restFileChunkSize];
                            buffer = new byte[filePathLen.Length + filePathByte.Length + restFileChunkSize];
                            filePathLen.CopyTo(buffer, 0);
                            filePathByte.CopyTo(buffer, filePathLen.Length);
                            file.Read(bytereadFile, 0, (int)restFileChunkSize);
                            bytereadFile.CopyTo(buffer, filePathLen.Length + filePathByte.Length);
                        }
                        else
                        {
                            bytereadFile = new byte[remainSize];
                            buffer = new byte[filePathLen.Length + filePathByte.Length + remainSize];
                            filePathLen.CopyTo(buffer, 0);
                            filePathByte.CopyTo(buffer, filePathLen.Length);
                            file.Read(bytereadFile, 0, (int)remainSize);
                            bytereadFile.CopyTo(buffer, filePathLen.Length + filePathByte.Length);
                        }



                    }
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(PostUrl);
                    webRequest.Method = "POST";
                    webRequest.ContentType = "application/octet-stream";
                    webRequest.ContentLength = buffer.Length;

                    Stream dataStream = webRequest.GetRequestStream();
                    dataStream.Write(buffer, 0, buffer.Length);
                    dataStream.Close();

                    //Response
                    HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

                    pos += restFileChunkSize;
                    remainSize -= restFileChunkSize;
                }
            }
            else
            {
                bytereadFile = System.IO.File.ReadAllBytes(f.FullName);
                buffer = new byte[filePathLen.Length + filePathByte.Length + bytereadFile.Length];

                filePathLen.CopyTo(buffer, 0);
                filePathByte.CopyTo(buffer, filePathLen.Length);
                bytereadFile.CopyTo(buffer, filePathLen.Length + filePathByte.Length);

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(PostUrl);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/octet-stream";
                webRequest.ContentLength = buffer.Length;

                Stream dataStream = webRequest.GetRequestStream();
                dataStream.Write(buffer, 0, buffer.Length);
                dataStream.Close();

                //Response
                HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
            }
            return true;
        }


        private PMLicenceKey GetSelectedItem()
        {
            KeyValuePair<string, string> itemSelected = (KeyValuePair<string, string>)cbCustomer.SelectedItem;
            return LicenceKeys.FirstOrDefault(x => x.PublicKey == itemSelected.Key);
        }

        private bool CreateFolderUploaded(string folder)
        {
            ServicePointManager.ServerCertificateValidationCallback = new
         RemoteCertificateValidationCallback
         (
             delegate { return true; }
         );
            try
            {
                string Url = Properties.Settings.Default.Url + "/api/Upload?Folder=" + folder;
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void LoadCustomers()
        {
            using (var db = new PMLicenceDevEntities())
            {
                LicenceKeys = db.PMLicenceKeys.OrderBy(x => x.PublicKey).ToList();
                if (LicenceKeys != null && LicenceKeys.Count > 0)
                {
                    List<KeyValuePair<string, string>> lstCus = new List<KeyValuePair<string, string>>();
                    foreach (var key in LicenceKeys)
                    {
                        KeyValuePair<string, string> cus = new KeyValuePair<string, string>(key.PublicKey, key.CusName);
                        lstCus.Add(cus);
                    }
                    cbCustomer.DataSource = lstCus;
                    cbCustomer.ValueMember = "Key";
                    cbCustomer.DisplayMember = "Value";
                    cbCustomer.SelectedIndex = 0;
                }
            }
        }

        private bool ValidateUpload()
        {
            bool flgResult = false;

            if (string.IsNullOrEmpty(txtLocalPath.Text))
                MessageBox.Show("Please select local path", "Information", MessageBoxButtons.OK);
            else if (!System.IO.Directory.Exists(txtLocalPath.Text))
                MessageBox.Show("Folder not exist", "Information", MessageBoxButtons.OK);
            else if (string.IsNullOrEmpty(txtUploadedVersion.Text))
                MessageBox.Show("Please type version", "Information", MessageBoxButtons.OK);
            else
                flgResult = true;

            return flgResult;
        }
        #endregion

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //for (int i = 1; i <= progressBar1.Maximum; i++)
            //{
            //    // Wait 100 milliseconds.
            //    Thread.Sleep(100);
            //    // Report progress.
            //    backgroundWorker1.ReportProgress(i);
            //}
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Change the value of the ProgressBar to the BackgroundWorker progress.
            progressBar1.Value = e.ProgressPercentage;
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtLocalPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
