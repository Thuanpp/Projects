using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Data;
using System.IO;
using System.ComponentModel;
using System.Xml;
using PMUpgrade.Model;
using System.Web;
using Newtonsoft.Json;
using System.Net.Security;

namespace PMUpgrade
{
    #region Declare Enums
    public enum UploadFileStatus
    {
        None,
        Done,
        InProcess,
        Error
    };
    #endregion

    public class PMFTPClientThread
    {

        #region Declare events
        public delegate void PMBrowseFolderEventHandler(object sender, PMFTPClientEventArgs e);
        public delegate void PMDownloadFileEventHandler(object sender, PMFTPClientEventArgs e);

        public event PMDownloadFileEventHandler PMDownloadFileFinish;
        public event PMDownloadFileEventHandler PMDownloadFileError;

        public event PMBrowseFolderEventHandler PMBrowseFolderFinished;
        public event PMBrowseFolderEventHandler PMBrowseFolderError;


        #endregion

        #region Property variables
        string m_SaveFilePath; // Absolute folder Path - for download file
        private MyFileInfo m_MyFileInfo;
        DataTable m_ListFolders;
        #endregion

        #region Local variables
        private Object gBrowseFolderLock;

        #endregion

        #region Constructors
        public PMFTPClientThread()
        {
            gBrowseFolderLock = new Object();
            ConfigDataTable("ListFolder");
        }
        #endregion

        public enum Verb
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        #region Properties
        public MyFileInfo MyFileInfo
        {
            get { return m_MyFileInfo; }
            set { m_MyFileInfo = value; }
        }

        public string SaveFilePath
        {
            get { return m_SaveFilePath; }
            set { m_SaveFilePath = value; }
        }

        public DataTable ListFolders
        {
            get { return m_ListFolders; }
            set { m_ListFolders = value; }
        }
        #endregion


        #region Private methods
        private void ConfigDataTable(string pTableName)
        {
            m_ListFolders = new DataTable(pTableName);
            m_ListFolders.Columns.Add("FileList", typeof(FileListInfo));
            m_ListFolders.Columns.Add("FullPath", typeof(string));
            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = m_ListFolders.Columns["FullPath"];
            m_ListFolders.PrimaryKey = PrimaryKeyColumns;
        }

        private void AddFolderToListFolders(string pFullPath, FileListInfo pFileListInfo)
        {
            DataRow dr = m_ListFolders.NewRow();
            dr["FullPath"] = pFullPath;
            dr["FileList"] = pFileListInfo;
            m_ListFolders.Rows.Add(dr);
        }


        private List<string> SendBrowseFolder(string rootPath, string maxVersion)
        {
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback
            (
                delegate { return true; }
            );
            List<string> listPath = new List<string>();
            try
            {
                string hostUrl = PMUpgrade.Properties.Settings.Default.HostUrl;
                string urlGetFolder = Path.Combine(hostUrl, "Folder");
                urlGetFolder += "?rootPath=" + rootPath + "&maxVersion=" + maxVersion;

                var request = (HttpWebRequest)WebRequest.Create(urlGetFolder);
                request.ContentType = "application/xml";

                var httpWebResponse = (HttpWebResponse)request.GetResponse();
                if (httpWebResponse.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = httpWebResponse.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();

                    httpWebResponse.Close();

                    var paths = JsonConvert.DeserializeObject<List<string>>(responseJson);

                    if (paths.Count > 0)
                    {
                        for (int i = 0; i < paths.Count; i++)
                        {
                            listPath.Add(paths[i]);
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            return listPath;
        }

        private FileListInfo GetFiles(string folderPath)
        {
            string hostUrl = PMUpgrade.Properties.Settings.Default.HostUrl;
            string urlGetFiles = Path.Combine(hostUrl, "Files") + "?folderPath=" + folderPath;
            var request = (HttpWebRequest)WebRequest.Create(urlGetFiles);
            request.ContentType = "application/xml";

            var httpWebResponse = (HttpWebResponse)request.GetResponse();
            FileListInfo fileListInfo = new FileListInfo();
            if (httpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string responseJson = reader.ReadToEnd();
                fileListInfo = JsonConvert.DeserializeObject<FileListInfo>(responseJson);
            }
            return fileListInfo;
        }

        private bool WriteFile(string rootPath, string maxVersion, MyFileInfo myFileInfo, byte[] buffer)
        {


            string fileName = myFileInfo.FileName.Substring(Path.Combine(rootPath, maxVersion).Length + 2);
            string fullPath = Path.Combine(m_SaveFilePath, fileName);

            if (File.Exists(fullPath))
            {
                using (var file = File.Open(fullPath, FileMode.Append, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(file)) // buffer size can be adjusted if necessary
                    {
                        writer.BaseStream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            else
            {
                string parentFolder = System.IO.Directory.GetParent(fullPath).FullName;

                if (!Directory.Exists(parentFolder))
                    Directory.CreateDirectory(parentFolder);
                
                using (var file = File.Open(fullPath, FileMode.CreateNew, FileAccess.Write))
                {
                    using (var writer = new StreamWriter(file)) // buffer size can be adjusted if necessary
                    {
                        writer.BaseStream.Write(buffer, 0, buffer.Length);
                    }
                }
            }

            return true;
        }

        private bool ProcessDownloadFile(string rootPath, string maxVersion, string postData, string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = Verb.POST.ToString();
                request.ContentType = "application/json";

                HttpWebResponse httpWebResponse = null;
                Stream dataStream;

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;
                dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                //Response
                httpWebResponse = (HttpWebResponse)request.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                MemoryStream ms = new MemoryStream();

                responseStream.CopyTo(ms);
                byte[] ret = ms.ToArray();
                WriteFile(rootPath, maxVersion, m_MyFileInfo, ret);
                httpWebResponse.Close();

            }
            catch
            {
                return false;
            }
          
            return true;
        }

        private bool SendDownLoadFile1(string rootPath, string maxVersion)
        {
            try
            {
                long restFileChunkSize = Utility.ConvertMegaBytesToBytes(
                                                PMUpgrade.Properties.Settings.Default.RestFileChunkSize);
                string hostUrl = PMUpgrade.Properties.Settings.Default.HostUrl;
                string urlContentFile = Path.Combine(hostUrl, "FileContent");
                FileContentInfo fileContentInfo = new FileContentInfo();

                if (m_MyFileInfo.TotalFileSize > restFileChunkSize)
                {

                    m_MyFileInfo.MultiplePart = true;
                    int chunkCount = (int)Math.Ceiling((double)m_MyFileInfo.TotalFileSize / (double)restFileChunkSize);
                    int count = 1;

                    do
                    {
                        m_MyFileInfo.FileSequence = count;
                        m_MyFileInfo.FileCount = chunkCount;
                        if (count == chunkCount)
                        {
                            m_MyFileInfo.CurrentFileSize = (int)(m_MyFileInfo.TotalFileSize - restFileChunkSize * (count - 1));
                        }
                        else
                        {
                            m_MyFileInfo.CurrentFileSize = (int)restFileChunkSize;
                        }

                        string postData = JsonConvert.SerializeObject(m_MyFileInfo);

                        //string postData = Utility.Serialize<MyFileInfo>(m_MyFileInfo);
                        //postData = postData.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                        //postData = postData.Replace("<MyFileInfo xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "<MyFileInfo xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/FTPServer.Models\">");

                        if (!ProcessDownloadFile(rootPath, maxVersion,postData, urlContentFile))
                        {
                            //error
                            return false;
                        }
                        count++;

                    } while (count <= chunkCount);
                }
                else
                {
                    //Request
                    m_MyFileInfo.MultiplePart = false;
                    string postData = JsonConvert.SerializeObject(m_MyFileInfo);
                    //string postData = Utility.Serialize<MyFileInfo>(m_MyFileInfo);
                    //postData = postData.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
                    //postData = postData.Replace("<MyFileInfo xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">", "<MyFileInfo xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://schemas.datacontract.org/2004/07/FTPServer.Models\">");
                    if (!ProcessDownloadFile(rootPath, maxVersion, postData, urlContentFile))
                    {
                        return false;
                    }

                }

            }
            catch (Exception ex)
            {
                //error
                return false;
            }
            return true;
        }
        #endregion

        #region Public methods
        public void BrowseFolder(string rootPath, string maxVersion)
        {
            try
            {
                PMFTPClientEventArgs clsPMFTPClientEventArgs = new PMFTPClientEventArgs(this);
                lock (gBrowseFolderLock)
                {
                    List<string> list = SendBrowseFolder(rootPath, maxVersion);

                    if (list == null)
                        throw new Exception("");

                    for (int i = 0; i < list.Count; i++)
                    {
                        FileListInfo fileListInfo = GetFiles(list[i]);
                        if (fileListInfo.Status.StatusCode != null
                             && fileListInfo.Status.StatusCode == HttpStatusCode.OK.ToString())
                        {
                            AddFolderToListFolders(list[i], fileListInfo);
                        }
                        else
                        {
                            throw new Exception("");
                        }
                    }
                }
                if (PMBrowseFolderFinished != null) PMBrowseFolderFinished(this, clsPMFTPClientEventArgs);
            }
            catch (Exception ex)
            {
                PMFTPClientEventArgs clsPMFTPClientEventArgs = new PMFTPClientEventArgs(this);
                if (PMBrowseFolderError != null) PMBrowseFolderError(this, clsPMFTPClientEventArgs);
            }
        }

        public void DownLoadFile(string rootPath, string maxVersion)
        {
            PMFTPClientEventArgs clsPMFTPClientEventArgs = new PMFTPClientEventArgs(this);
            if (SendDownLoadFile1(rootPath, maxVersion))
            {
                if (PMDownloadFileFinish != null) PMDownloadFileFinish(this, clsPMFTPClientEventArgs);
            }
            else
            {
                if (PMDownloadFileError != null) PMDownloadFileError(this, clsPMFTPClientEventArgs);
            }
        }
        #endregion
    }

    public class PMFTPClientEventArgs
    {
        #region Fields
        private PMFTPClientThread m_PMFTPClientThread;
        private int m_ErrorID;
        //private string m_Descriptions;
        private string m_ErrorMessage;
        private string m_TransFileName;
        private int m_PercentCompleted;
        #endregion

        #region Properties
        public PMFTPClientThread PMFTPClientThread
        {
            get { return m_PMFTPClientThread; }
        }

        internal int ErrorID
        {
            get { return m_ErrorID; }
            set { m_ErrorID = value; }
        }

        internal string ErrorMessage
        {
            get { return m_ErrorMessage; }
            set { m_ErrorMessage = value; }
        }

        internal int PercentCompleted
        {
            get { return m_PercentCompleted; }
            set { m_PercentCompleted = value; }
        }

        internal string TransFileName
        {
            get { return m_TransFileName; }
            set { m_TransFileName = value; }
        }
        #endregion

        #region Constructor and Disposable
        public PMFTPClientEventArgs()
        {

        }

        public PMFTPClientEventArgs(PMFTPClientThread pclsPMFTPClientThread)
        {
            m_PMFTPClientThread = pclsPMFTPClientThread;
        }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        public int GetErrorID()
        {
            return m_ErrorID;
        }

        public string GetDescription()
        {
            return m_ErrorMessage;
        }

        public int GetPercentCompleted()
        {
            return m_PercentCompleted;
        }

        public string GetTransFileName()
        {
            return m_TransFileName;
        }
        #endregion
    }
}



