using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using PMUpgrade.Model;

namespace PMUpgrade
{
    public partial class Form1 : Form
    {
        private PMFTPClientThread gclsPMFTPClientThread;
        private DataTable dt;
        private bool flgGetFile;
        private List<string> lstPath;
        private const string FolderUpdateName = "UpgradePMFolder";
        private const string FolderBackupName = "BackupPMFolder";
        public string RootPath { get; set; }
        public string MaxVersion { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string rootPath, string maxVersion)
        {
            RootPath = rootPath;
            MaxVersion = maxVersion;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.btnStart.Enabled = true;
            this.btnRetry.Enabled = false;
            gclsPMFTPClientThread = new PMFTPClientThread();
            gclsPMFTPClientThread.PMBrowseFolderFinished += new PMFTPClientThread.PMBrowseFolderEventHandler(gclsPMFTPClientThread_PMBrowseFolderFinished);
            gclsPMFTPClientThread.PMBrowseFolderError += new PMFTPClientThread.PMBrowseFolderEventHandler(gclsPMFTPClientThread_PMBrowseFolderError);

            gclsPMFTPClientThread.PMDownloadFileFinish += new PMFTPClientThread.PMDownloadFileEventHandler(gclsPMFTPClientThread_PMDownloadFileFinish);
            gclsPMFTPClientThread.PMDownloadFileError += new PMFTPClientThread.PMDownloadFileEventHandler(gclsPMFTPClientThread_PMDownloadFileError);
        }

        private void gclsPMFTPClientThread_PMBrowseFolderFinished(object sender, PMFTPClientEventArgs e)
        {
            try
            {
                dt = e.PMFTPClientThread.ListFolders;
                flgGetFile = true;
            }
            catch
            { }
            finally
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        this.Cursor = Cursors.Default;
                    }));
                }
                else this.Cursor = Cursors.Default;
            }
        }

        private void gclsPMFTPClientThread_PMBrowseFolderError(object sender, PMFTPClientEventArgs e)
        {
            flgGetFile = false;
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        this.Cursor = Cursors.Default;
                    }));
                }
                else this.Cursor = Cursors.Default;
            }
            catch
            { }
        }

        private void gclsPMFTPClientThread_PMDownloadFileFinish(object send, PMFTPClientEventArgs e)
        {
            try
            {
                flgGetFile = true;
                progressBar1.Value++;
            }
            catch (Exception ex)
            {
            }
        }

        private void gclsPMFTPClientThread_PMDownloadFileError(object sender, PMFTPClientEventArgs e)
        {
            try
            {
                flgGetFile = false;
            }
            catch
            { }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                dt = null;
                flgGetFile = false;
                this.btnStart.Enabled = false;
                lblStatus.Text = "Đang tải...";
                Application.DoEvents();

                gclsPMFTPClientThread.ListFolders.Clear();
                gclsPMFTPClientThread.BrowseFolder(RootPath, MaxVersion);
                if (flgGetFile)
                {
                    if (dt.Rows.Count > 0)
                    {
                        int count = 0;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            FileListInfo fileListInfo = (FileListInfo)dt.Rows[i]["FileList"];
                            if (fileListInfo != null)
                            {
                                if (fileListInfo.MyFileInfoList != null)
                                    count += fileListInfo.MyFileInfoList.Count;
                            }

                        }

                        progressBar1.Value = 0;
                        progressBar1.Maximum = count;

                        string curPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        string parentFolder = System.IO.Directory.GetParent(curPath).FullName;
                        gclsPMFTPClientThread.SaveFilePath = Path.Combine(parentFolder, FolderUpdateName);
                        if (Directory.Exists(gclsPMFTPClientThread.SaveFilePath))
                        {
                            Directory.Delete(gclsPMFTPClientThread.SaveFilePath, true);
                        }
                        Directory.CreateDirectory(gclsPMFTPClientThread.SaveFilePath);

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            FileListInfo fileListInfo = (FileListInfo)dt.Rows[i]["FileList"];
                            if (fileListInfo != null)
                            {
                                if (fileListInfo.MyFileInfoList != null && fileListInfo.MyFileInfoList.Count > 0)
                                {
                                    foreach(var item in fileListInfo.MyFileInfoList)
                                    {
                                        gclsPMFTPClientThread.MyFileInfo = item;
                                        gclsPMFTPClientThread.DownLoadFile(RootPath, MaxVersion);
                                        if (!flgGetFile) throw new Exception("");
                                    }
                                }
                            }
                        }

                        System.Threading.Thread.Sleep(500);
                        lblStatus.Text = "tải hoàn thành";
                        Application.DoEvents();

                        if (IsProcessOpen("PMUserInterface"))
                        {
                            DialogResult dialogResult = MessageBox.Show(@"Muốn cập nhật phải đóng chương trình quản lý dự án?" + System.Environment.NewLine +
                                                                            "                        Bạn có muốn đóng không?", "Thông báo", MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                Process[] procs = null;
                                procs = Process.GetProcessesByName("PMUserInterface");
                                Process PMUserInterfaceProc = procs[0];
                                if (!PMUserInterfaceProc.HasExited)
                                {
                                    PMUserInterfaceProc.Kill();
                                }
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                MessageBox.Show("Đã hoãn cập nhật");
                                btnStart.Enabled = true;
                                btnRetry.Enabled = false;
                                progressBar1.Value = 0;
                                lblStatus.Text = "";
                                Application.DoEvents();
                                return;
                            }


                        }

                        lblStatus.Text = "đang cập nhật...";
                        Application.DoEvents();


                        // backup old files 
                        string[] lstoldFiles = System.IO.Directory.GetFiles(parentFolder, "*.*", System.IO.SearchOption.TopDirectoryOnly);
                        string[] lstnewFiles = System.IO.Directory.GetFiles(gclsPMFTPClientThread.SaveFilePath, "*.*", System.IO.SearchOption.TopDirectoryOnly);
                        int cntoldFiles = lstoldFiles.Count();
                        int cntnewFiles = lstnewFiles.Count();
                        progressBar1.Value = 0;
                        progressBar1.Maximum = cntoldFiles + cntnewFiles;

                        if (cntoldFiles > 0)
                        {
                            string pathBackFolder = Path.Combine(parentFolder, FolderBackupName);
                            if (Directory.Exists(pathBackFolder))
                            {
                                Directory.Delete(pathBackFolder, true);
                            }
                            Directory.CreateDirectory(pathBackFolder);

                            for (int i = 0; i < cntoldFiles; i++)
                            {
                                string fileName = Path.GetFileName(lstoldFiles[i]);
                                File.Move(lstoldFiles[i], Path.Combine(pathBackFolder, fileName));
                                progressBar1.Value++;
                            }
                        }

                        //update new files
                        cntnewFiles = lstnewFiles.Count();
                        if (cntnewFiles > 0)
                        {
                            for (int i = 0; i < cntnewFiles; i++)
                            {
                                string fileName = Path.GetFileName(lstnewFiles[i]);
                                File.Copy(lstnewFiles[i], Path.Combine(parentFolder, fileName));
                                progressBar1.Value++;
                            }
                        }

                        lblStatus.Text = "Cập nhật thành công";
                        Application.DoEvents();

                        //IUIniFile.IniFileStrSetting("IsUpGradge", "Flag", "1", IUIniFile.fnc_ReturnMainPath() + "Setting.ini");
                        System.Threading.Thread.Sleep(500);

                        DialogResult dialogResult1 = MessageBox.Show(@"Bạn có muốn chạy chương trình sau khi cập nhật không?", "Thông báo", MessageBoxButtons.YesNo);
                        if (dialogResult1 == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(Path.Combine(parentFolder, "PMUserInterface.exe"));
                            Application.Exit();
                        }
                        else if (dialogResult1 == DialogResult.No)
                        {
                            Application.Exit();
                        }

                    }
                }
                else
                {
                    if (!flgGetFile) throw new Exception("");
                }


            }
            catch (Exception Ex)
            {
                MessageBox.Show(@"Đã có lỗi xảy ra " + Ex.Message + ".Hãy thử lại", "Upgrade", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.btnRetry.Enabled = true;
            }
        }

        public bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Equals(name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
