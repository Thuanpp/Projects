using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using Utility;
using System.Net;
using System.Net.Security;
using System.Diagnostics;

namespace MocauManagement
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            //FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            //string version = fvi.FileVersion;

            if (string.IsNullOrEmpty(txtFolderPath.Text.Trim())) return;

            string folderUploaded = new DirectoryInfo(txtFolderPath.Text.Trim()).Name;
            if (!CreateFolderUploaded(folderUploaded))
            {
                MessageBox.Show("Error during upload");
                return;
            }

            int ChunkSize = Properties.Settings.Default.restFileChunkSize;
            long restFileChunkSize = Utility.UtilityConvert.ConvertMegaBytesToBytes(double.Parse(ChunkSize.ToString()));

            string[] files = Directory.GetFiles(txtFolderPath.Text, "*.*", SearchOption.AllDirectories);
            foreach (string f in files)
            {
                if (!File.Exists(f)) continue;

                try
                {
                    FileInfo fInfo = new FileInfo(f);
                    UploadFile(fInfo, restFileChunkSize, folderUploaded);
                }
                catch
                {
                    MessageBox.Show("Error during upload");
                    return;
                }
            }
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
                string PostUrl = Properties.Settings.Default.Url + "/api/Upload?Folder=" + folder;
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(PostUrl);
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }            
        }

        private bool UploadFile(FileInfo f, long restFileChunkSize, string folderUploaded)
        {

            ServicePointManager.ServerCertificateValidationCallback = new
           RemoteCertificateValidationCallback
           (
               delegate { return true; }
           );

            long pos = 0;
            long remainSize = f.Length;
            string relativePath = f.FullName.Substring(txtFolderPath.Text.Trim().Length - folderUploaded.Length);
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

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFolderPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
