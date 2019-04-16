using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using PMUserInterface.PMFunctions;

namespace TCPServer
{
    public partial class PMMain : Form
    {
        MKServer.SynchronousSocketListener clsSynchronousSocketListener;
        private Thread t_listener;
        private string m_ServerIP;
        
        public PMMain()
        {
            InitializeComponent();

            this.cboIP.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            btnStartServer.Enabled = false;
            btnStopServer.Enabled = true;
            StartTcpServer();
        }

        public string GetIP()
        {
            string name = Dns.GetHostName();
            IPHostEntry entry = Dns.GetHostEntry(name);
            IPAddress[] addr = entry.AddressList;
            for (int i = 0; i < addr.Length; i++)
            {
                if (addr[i].ToString().Split('.').Length == 4)
                {
                    this.cboIP.Items.Add(addr[i]);
                }
            }
            if (this.cboIP.Items.Count > 0) this.cboIP.SelectedIndex = 0;
            return this.cboIP.Text;
            //if (addr[1].ToString().Split('.').Length == 4)
            //{
            //    return addr[1].ToString();
            //}
            //return addr[2].ToString();
        }

        public void StartTcpServer()
        {
            m_ServerIP = this.cboIP.Text;
            clsSynchronousSocketListener = new MKServer.SynchronousSocketListener();
            clsSynchronousSocketListener.ServerPath = this.txtPathFolder.Text;
            clsSynchronousSocketListener.ServerIpAddress = m_ServerIP;
            clsSynchronousSocketListener.ServerPort = 8085;
            clsSynchronousSocketListener.StartServer = true;
            t_listener = new Thread(clsSynchronousSocketListener.StartListening);
            t_listener.Start();
        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            btnStopServer.Enabled = false;
            btnStartServer.Enabled = true;
            StopListiening();
        }

        private void StopListiening()
        {
            if (clsSynchronousSocketListener != null) clsSynchronousSocketListener.StopListening();
            clsSynchronousSocketListener = null;

            if (t_listener != null)
            {
                if (t_listener.IsAlive) t_listener.Abort();
                t_listener = null;
            }
        }

        private void PMMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (clsAsyncSocketListener != null) clsAsyncSocketListener.Dispose();
            //clsAsyncSocketListener = null;
            if (clsSynchronousSocketListener != null) StopListiening();
        }

        private void PMMain_Load(object sender, EventArgs e)
        {
            try
            {
                string sFolderPath = IUIniFile.IniFileStrGetting("TCPServer", "PathFolder", "", IUIniFile.fnc_ReturnMainPath() + "Setting.ini");
                this.txtPathFolder.Text = sFolderPath;
                m_ServerIP = GetIP();
                this.chkRunAtWindowsStartUp.Checked = true;
                StartTcpServer();
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.BalloonTipText = "PMServer";
                notifyIcon1.BalloonTipTitle = "PMServer";
                notifyIcon1.ShowBalloonTip(1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"PMMain\PMMain_Load Error: " + ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (clsAsyncSocketListener != null) clsAsyncSocketListener.Dispose();
            //clsAsyncSocketListener = null;
            if (clsSynchronousSocketListener != null) StopListiening();
            Application.Exit();
        }

        private void chkRunAtWindowsStartUp_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkRunAtWindowsStartUp.Checked)
                {
                    RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    regKey.SetValue("PMServer", Application.ExecutablePath);
                }
                else
                {
                    RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    regKey.DeleteValue("PMServer");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"PMMain\chkRunAtWindowsStartUp_CheckedChanged Error: " + ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fDialog = new FolderBrowserDialog();
                DialogResult dr = fDialog.ShowDialog();
                if (dr == System.Windows.Forms.DialogResult.OK)
                {
                    this.txtPathFolder.Text = fDialog.SelectedPath;
                    //START 2018/01/17 Vuongnv ADD
                    IUIniFile.IniFileStrSetting("TCPServer", "PathFolder", this.txtPathFolder.Text, IUIniFile.fnc_ReturnMainPath() + "Setting.ini");
                    //END 2018/01/17 Vuongnv ADD
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"PMMain\btnBrowse_Click Error: " + ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void cboIP_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //m_ServerIP = this.cboIP.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"PMMain\cboIP_SelectedIndexChanged Error: " + ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                m_ServerIP = this.cboIP.Text;
                if (clsSynchronousSocketListener != null)
                {
                    clsSynchronousSocketListener.ServerPath = this.txtPathFolder.Text;
                    clsSynchronousSocketListener.ServerIpAddress = m_ServerIP;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"PMMain\btnApply_Click Error: " + ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }


}
