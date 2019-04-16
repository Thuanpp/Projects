/*=================================================================
Project                :  
Package                :  
Module                 :  
Description            :  
Start Date             :  08/07/2017
Finish Date            :  08/07/2017
Last Update            :  
Class Name             :  MKTranferFile
=================================================================
=================================================================
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace MKServer
{
    public class MKTranferFile : IDisposable
    {
        #region Propertiy variables
        private Thread m_Thread;
        private TcpClient m_Client;

        private string m_FileName;
        private string m_FolderPath;
        #endregion

        #region Local variables
        private bool disposeCalled;
        #endregion

        #region Constructors
        public MKTranferFile()
        {
            disposeCalled = false;
        }

        public MKTranferFile(TcpClient pClient)
            : this()
        {
            m_FolderPath = @"D:\\Downloads\\";
            m_Client = pClient;
            m_Client.ReceiveTimeout = 21600000; // 21.600.000ms -> 1h
            m_Thread = new Thread(new ThreadStart(StartFTP));
        }
        #endregion

        #region Properties
        public TcpClient Client
        {
            get { return m_Client; }
            set { m_Client = value; }
        }

        public string FolderPath
        {
            get { return m_FolderPath; }
            set { m_FolderPath = value; }
        }

        public string FileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }
        #endregion

        #region Private Methods
        private void StartFTP()
        {
            byte[] buffer = new byte[1500];

            /* tạo object truyền nhận lệnh với client */
            NetworkStream ns = m_Client.GetStream();
            NetworkStream sw = m_Client.GetStream();

            /* chờ nhận lệnh từ client */
            int iBytesread = ns.Read(buffer, 0, buffer.Length);

            /* đã nhận được lệnh từ client */
            string sCommandRev = Encoding.UTF8.GetString(buffer, 0, iBytesread);
            string sCommandSend = string.Empty;

            /* phân tích lệnh */
            switch (sCommandRev)
            {
                case "GET": // download file
                    sCommandSend = "ACK";
                    int bytesCommand = sCommandSend.Length;
                    byte[] commandByte = Encoding.UTF8.GetBytes(sCommandSend);

                    /* gửi lệnh đồng ý về client */
                    sw.Write(commandByte, 0, bytesCommand);

                    /* gửi nội dung file về client */
                    SendFile();

                    break;
                case "PUT": // upload file
                    sCommandSend = "ACK";
                    int bytesCommandPut = sCommandSend.Length;
                    byte[] commandBytePut = Encoding.UTF8.GetBytes(sCommandSend);

                    /* gửi lệnh đồng ý về client */
                    sw.Write(commandBytePut, 0, bytesCommandPut);

                    /* gửi nội dung file về client */
                    ReceiveFile();

                    break;

                case "DEL": // Delete file
                    sCommandSend = "ACK";
                    int bytesCommandDel = sCommandSend.Length;
                    byte[] commandByteDel = Encoding.UTF8.GetBytes(sCommandSend);

                    /* gửi lệnh đồng ý về client */
                    sw.Write(commandByteDel, 0, bytesCommandDel);

                    DeleteFile();

                    break;

                case "FIN": // duyệt cây thư mục
                    sCommandSend = "ACK";
                    int bytesCommandFind = sCommandSend.Length;
                    byte[] commandByteFind = Encoding.UTF8.GetBytes(sCommandSend);

                    /* gửi lệnh đồng ý về client */
                    sw.Write(commandByteFind, 0, bytesCommandFind);
                    FindPaths();
                    break;

                case "CRD": // Tạo Thư mục
                    sCommandSend = "ACK";
                    int bytesCommandCreate = sCommandSend.Length;
                    byte[] commandByteCreate = Encoding.UTF8.GetBytes(sCommandSend);

                    /* gửi lệnh đồng ý về client */
                    sw.Write(commandByteCreate, 0, bytesCommandCreate);
                    CreateFolder();
                    break;


            }
        }

        private void ReceiveFile()
        {
            string sError = string.Empty;
            try
            {
                byte[] buffer = new byte[1500];

                NetworkStream sr = m_Client.GetStream();

                /* nhận kích thước file, kích thước tên file và tên file của khối truyền đầu tiên */
                int iBytesread = sr.Read(buffer, 0, buffer.Length);

                /* 8 bytes nhận đầu tiên trong buffer là kích thước của file */
                long lFileSize = BitConverter.ToInt64(buffer, 0);

                /* kế tiếp là vị trí của fileNameLen */
                int fileNameLen = BitConverter.ToInt32(buffer, 8);

                /* sau đó là vị trí của fileName (8 + 4 = 12) */
                string sRelativeFilePath = Encoding.UTF8.GetString(buffer, 12, fileNameLen);

                /* tách projectID và file name */
                //Char delimiter = '\\';
                //String[] substrings = m_FileName.Split(delimiter);
                //string sProjectID = substrings[0];
                //m_FileName = substrings[1];
                //m_FolderPath = System.IO.Path.Combine(m_FolderPath, sProjectID);
                string sFilePathFull = System.IO.Path.Combine(m_FolderPath, sRelativeFilePath);
                m_FileName = System.IO.Path.GetFileName(sFilePathFull);
                string sAbsoluteFolderPath = System.IO.Path.GetDirectoryName(sFilePathFull);
                if (!System.IO.Directory.Exists(sAbsoluteFolderPath)) System.IO.Directory.CreateDirectory(sAbsoluteFolderPath);

                //string sPath = System.IO.Path.Combine(m_FolderPath, m_FileName); // m_FolderPath + m_FileName;
                int iNumeric = 0;
                string extension;
                string sCurFileName = m_FileName;
                while (System.IO.File.Exists(sFilePathFull))
                {
                    iNumeric++;
                    extension = Path.GetExtension(m_FileName);
                    sCurFileName = m_FileName.Replace(extension, "(" + iNumeric.ToString() + ")" + extension);
                    sFilePathFull = System.IO.Path.Combine(sAbsoluteFolderPath, sCurFileName); // m_FolderPath + sCurFileName;
                }

                StreamWriter writer = new StreamWriter(sFilePathFull);

                int iHeaderSize = 12 + fileNameLen; // m_FileName.Length;
                long lbytesRevceive = 0;
                if (iHeaderSize < iBytesread)
                {
                    lbytesRevceive = iBytesread - iHeaderSize;
                    writer.BaseStream.Write(buffer, iHeaderSize, iBytesread - iHeaderSize);
                }

                int bytesread = 0;
                while (lbytesRevceive < lFileSize)
                {
                    bytesread = sr.Read(buffer, 0, buffer.Length);
                    lbytesRevceive += bytesread;
                    if (lbytesRevceive > lFileSize) break;
                    writer.BaseStream.Write(buffer, 0, bytesread);
                }

                //NetworkStream sw = m_Client.GetStream();
                //string sCommand = "FIN";
                //int bytesCommand = sCommand.Length;
                //byte[] commandByte = Encoding.UTF8.GetBytes(sCommand);
                //sw.Write(commandByte, 0, bytesCommand);

                NetworkStream sw = m_Client.GetStream();
                int sCommand = 101;
                byte[] commandByte = BitConverter.GetBytes(sCommand);
                byte[] pathByte = Encoding.UTF8.GetBytes(sCurFileName/*sFilePathFull*/);
                byte[] pathLen = BitConverter.GetBytes(pathByte.Length);

                commandByte.CopyTo(buffer, 0);
                pathLen.CopyTo(buffer, commandByte.Length);
                pathByte.CopyTo(buffer, commandByte.Length + pathLen.Length);
                sw.Write(buffer, 0, commandByte.Length + commandByte.Length + pathByte.Length);

                writer.Close();
            }
            catch { }
            finally
            {
                if (m_Client != null) m_Client.Close();
                m_Client = null;
                if (m_Thread != null) m_Thread.Abort();
                m_Thread = null;
            }
        }

        private void SendFile()
        {
            StreamReader srReadFile = null;
            try
            {
                byte[] buffer = new byte[1500];

                /* tạo object truyền nhận lệnh với client */
                NetworkStream sr = m_Client.GetStream();
                NetworkStream sw = m_Client.GetStream();

                /* chờ nhận thông tin file */
                int iBytesread = sr.Read(buffer, 0, buffer.Length);

                /* lấy 4 bytes dầu tiên là kích thước của file path */
                int fileNameLen = BitConverter.ToInt32(buffer, 0);

                /* lấy file path bắt đầu từ vị trí thứ 4 */
                string sFilePathFull = Encoding.UTF8.GetString(buffer, 4, fileNameLen);
                string sPathFull = System.IO.Path.Combine(m_FolderPath, sFilePathFull);

                /* nếu file không tồn tại */
                int iCommandIndex = 0;
                if (!System.IO.File.Exists(sPathFull))
                {
                    /* gửi lệnh file không tồn tại về client */
                    //string sCommand = "NOTF";
                    //byte[] commandByte = Encoding.UTF8.GetBytes(sCommand);
                    iCommandIndex = 100; // lệnh error
                    byte[] commandByte = BitConverter.GetBytes(iCommandIndex);
                    sw.Write(commandByte, 0, commandByte.Length);

                    return;
                }

                /* nếu file tồn tại, gửi lệnh đồng ý về client */
                //string sCommandOK = "ACK";
                //byte[] commandByteOK = Encoding.UTF8.GetBytes(sCommandOK);
                iCommandIndex = 101; // lệnh OK
                byte[] commandByteOK = BitConverter.GetBytes(iCommandIndex);
                commandByteOK.CopyTo(buffer, 0);
                //sw.Write(commandByteOK, 0, commandByteOK.Length);

                /* mở file */
                srReadFile = new StreamReader(sPathFull);

                long lFileSize = srReadFile.BaseStream.Length;
                byte[] fileLen = BitConverter.GetBytes(lFileSize);
                fileLen.CopyTo(buffer, commandByteOK.Length);

                /* truyền kích thước file về client */
                sw.Write(buffer, 0, commandByteOK.Length + fileLen.Length);

                /* chờ nhận thông tin client */
                iBytesread = sr.Read(buffer, 0, buffer.Length);
                iCommandIndex = BitConverter.ToInt32(buffer, 0);
                if (iCommandIndex != 101) // not OK
                {
                    return;
                }

                /* truyền nội dung file */
                long bytesSent = 0;
                int bytesRead = 0;
                while (bytesSent < lFileSize)
                {
                    bytesRead = srReadFile.BaseStream.Read(buffer, 0, 1500);
                    sw.Write(buffer, 0, bytesRead);
                    bytesSent += bytesRead;
                }
            }
            catch { }
            finally
            {
                //System.Diagnostics.Debug.WriteLine(iNumber);
                if (srReadFile != null) srReadFile.Close();
                srReadFile = null;
                if (m_Client != null) m_Client.Close();
                m_Client = null;
                if (m_Thread != null) m_Thread.Abort();
                m_Thread = null;
            }

        }

        private void DeleteFile()
        {
            try
            {
                byte[] buffer = new byte[1500];

                /* tạo object truyền nhận lệnh với client */
                NetworkStream sr = m_Client.GetStream();
                NetworkStream sw = m_Client.GetStream();
                /* chờ nhận thông tin file */
                int iBytesread = sr.Read(buffer, 0, buffer.Length);

                /* lấy 4 bytes dầu tiên là kích thước của file or folder path */
                int fileOrFolderNameLen = BitConverter.ToInt32(buffer, 0);

                /* lấy file or folder path bắt đầu từ vị trí thứ 4 */
                string sPathFull = Encoding.UTF8.GetString(buffer, 4, fileOrFolderNameLen);

                m_FolderPath = System.IO.Path.Combine(m_FolderPath, sPathFull);
                /* nếu file không tồn tại */
                int iCommandIndex = 0;
                if (!System.IO.File.Exists(m_FolderPath) && !Directory.Exists(m_FolderPath))
                {
                    /* gửi lệnh file không tồn tại về client */
                    iCommandIndex = 100; // lệnh error
                    byte[] commandByte = BitConverter.GetBytes(iCommandIndex);
                    sw.Write(commandByte, 0, commandByte.Length);
                }
                else
                {
                    try
                    {
                        if (System.IO.File.Exists(m_FolderPath)) System.IO.File.Delete(m_FolderPath);
                        else Directory.Delete(m_FolderPath, true);
                        /* gửi lệnh đã xóa file thành công */
                        iCommandIndex = 101; // lệnh OK
                        byte[] commandByteOK = BitConverter.GetBytes(iCommandIndex);
                        sw.Write(commandByteOK, 0, commandByteOK.Length);
                    }
                    catch
                    {
                        iCommandIndex = 102; // can't delele
                        byte[] commandByteNotDelete = BitConverter.GetBytes(iCommandIndex);
                        sw.Write(commandByteNotDelete, 0, commandByteNotDelete.Length);

                    }
                }
            }
            catch
            {
            }
            finally
            {
                //System.Diagnostics.Debug.WriteLine(iNumber);
                if (m_Client != null) m_Client.Close();
                m_Client = null;
                if (m_Thread != null) m_Thread.Abort();
                m_Thread = null;
            }

        }

        private void FindPaths()
        {
            try
            {
                List<string> listPath = null;
                byte[] buffer = new byte[1500];

                /* tạo object truyền nhận lệnh với client */
                NetworkStream sr = m_Client.GetStream();
                NetworkStream sw = m_Client.GetStream();
                /* chờ nhận thông tin file */
                int iBytesread = sr.Read(buffer, 0, buffer.Length);

                /* lấy 4 bytes dầu tiên là kích thước của file or folder path */
                int FolderNameLen = BitConverter.ToInt32(buffer, 0);

                /* lấy file or folder path bắt đầu từ vị trí thứ 4 */
                string sPathFull = Encoding.UTF8.GetString(buffer, 4, FolderNameLen);

                m_FolderPath = System.IO.Path.Combine(m_FolderPath, sPathFull);
                /* nếu file không tồn tại */
                int iCommandIndex = 0;
                if (!Directory.Exists(m_FolderPath))
                {
                    /* gửi lệnh file không tồn tại về client */
                    iCommandIndex = 100; // lệnh error
                    byte[] commandByte = BitConverter.GetBytes(iCommandIndex);
                    sw.Write(commandByte, 0, commandByte.Length);
                }
                else
                {
                    try
                    {
                        listPath = new List<string>();
                        TraverseTree(m_FolderPath, listPath);
                        if (listPath.Count > 0)
                        {
                            /* gửi lệnh Found */
                            iCommandIndex = 101; // lệnh OK
                            byte[] commandByteOK = BitConverter.GetBytes(iCommandIndex);
                            commandByteOK.CopyTo(buffer, 0);


                            long totalLen = 0;
                            for (int i = 0; i < listPath.Count; i += 4)
                            {
                                byte[] byteItem = Encoding.UTF8.GetBytes(listPath[i]); // mảng byte tên file
                                byte[] byteLenItem = BitConverter.GetBytes(byteItem.Length); // mảng byte chiều dài tên file
                                totalLen += byteLenItem.Length + byteItem.Length + 12 + 1 + 1; // 12 là length của yyMMddHHmmss, 1 là Folder Or file (0 or 1), 1 kiem tra co subfolder hay khong
                            }
                            //foreach (string item in listPath)
                            //{
                            //    byte[] byteItem = Encoding.UTF8.GetBytes(item);
                            //    byte[] byteLenItem = BitConverter.GetBytes(byteItem.Length);
                            //    totalLen += byteLenItem.Length + byteItem.Length;
                            //}
                            byte[] commandByteCount = BitConverter.GetBytes(totalLen);
                            commandByteCount.CopyTo(buffer, commandByteOK.Length);
                            sw.Write(buffer, 0, commandByteOK.Length + commandByteCount.Length);

                            iBytesread = sr.Read(buffer, 0, buffer.Length);
                            if (Encoding.UTF8.GetString(buffer, 0, iBytesread) == "OK")
                            {
                                byte[] byteResult = new byte[totalLen];
                                totalLen = 0;
                                for (int i = 0; i < listPath.Count; i += 4)
                                {
                                    byte[] byteItem = Encoding.UTF8.GetBytes(listPath[i]);
                                    byte[] byteLenItem = BitConverter.GetBytes(byteItem.Length);
                                    byte[] byteCreationTime = Encoding.UTF8.GetBytes(listPath[i + 1]); 
                                    byte[] byteFolderOrFile = Encoding.UTF8.GetBytes(listPath[i + 2]);
                                    byte[] byteHaveSubFolder = Encoding.UTF8.GetBytes(listPath[i + 3]);
                                    byteLenItem.CopyTo(byteResult, totalLen);
                                    byteItem.CopyTo(byteResult, totalLen + byteLenItem.Length);
                                    byteCreationTime.CopyTo(byteResult, totalLen + byteLenItem.Length + byteItem.Length);
                                    byteFolderOrFile.CopyTo(byteResult, totalLen + byteLenItem.Length + byteItem.Length + 12);
                                    byteHaveSubFolder.CopyTo(byteResult, totalLen + byteLenItem.Length + byteItem.Length + 12 + 1);
                                    totalLen += byteLenItem.Length + byteItem.Length + 12 + 1 + 1;
                                }
                                //foreach (string item in listPath)
                                //{
                                //    byte[] byteItem = Encoding.UTF8.GetBytes(item);
                                //    byte[] byteLenItem = BitConverter.GetBytes(byteItem.Length);
                                //    byteLenItem.CopyTo(byteResult, totalLen);
                                //    byteItem.CopyTo(byteResult, totalLen + byteLenItem.Length);
                                //    totalLen += byteLenItem.Length + byteItem.Length;
                                //}
                                iBytesread = 0;
                                while (totalLen > 0)
                                {
                                    if (totalLen >= 1500)
                                    {
                                        sw.Write(byteResult, iBytesread, 1500);
                                    }
                                    else
                                    {
                                        sw.Write(byteResult, iBytesread, (int)totalLen);
                                    }
                                    totalLen -= 1500;
                                    iBytesread += 1500;
                                }
                            }
                        }
                        else
                        {
                            /* gửi lệnh Thư mục rỗng */
                            iCommandIndex = 103; // lệnh OK
                            byte[] commandByteOK = BitConverter.GetBytes(iCommandIndex);
                            sw.Write(commandByteOK, 0, commandByteOK.Length);
                        }
                    }
                    catch
                    {
                        iCommandIndex = 102; // Not Found
                        byte[] commandByteNotFound = BitConverter.GetBytes(iCommandIndex);
                        sw.Write(commandByteNotFound, 0, commandByteNotFound.Length);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                //System.Diagnostics.Debug.WriteLine(iNumber);
                if (m_Client != null) m_Client.Close();
                m_Client = null;
                if (m_Thread != null) m_Thread.Abort();
                m_Thread = null;
            }

        }

        //Duyệt Cấp 1
        private void TraverseTree(string root, List<string> listPath)
        {
            string[] subDirs;
            try
            {
                subDirs = System.IO.Directory.GetDirectories(root);
            }
            catch// (UnauthorizedAccessException e)
            {
                return;
            }
            //catch (System.IO.DirectoryNotFoundException e)
            //{
            //    return;
            //}

            string[] files = null;
            try
            {
                files = System.IO.Directory.GetFiles(root);
            }

            catch// (UnauthorizedAccessException e)
            {
                return;
            }
            //catch (System.IO.DirectoryNotFoundException e)
            //{
            //    return;
            //}

            // Perform the required action on each file here.
            // Modify this block to perform your required task.
            foreach (string subDir in subDirs)
            {
                try
                {
                    // Perform whatever action is required in your scenario.
                    System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(subDir);
                    listPath.Add(dir.Name);
                    listPath.Add(dir.CreationTime.ToString("yyMMddHHmmss"));
                    listPath.Add("0"); // Folder
                    string[] sub2Dirs = System.IO.Directory.GetDirectories(dir.FullName);
                    if (sub2Dirs.Length > 0)
                    {
                        listPath.Add("1"); // Folder have subfolder
                    }
                    else
                    {
                        string[] filesOfSubFolder = System.IO.Directory.GetFiles(dir.FullName);
                        if (filesOfSubFolder.Length > 0)
                            listPath.Add("1"); // Folder have file
                        else
                            listPath.Add("0"); // Folder not have SubFolder And file
                    }
                }
                catch (System.IO.FileNotFoundException e)
                {
                    // If file was deleted by a separate application
                    //  or thread since the call to TraverseTree()
                    // then just continue.
                    //Console.WriteLine(e.Message);
                    continue;
                }
            }

            foreach (string file in files)
            {
                try
                {
                    // Perform whatever action is required in your scenario.
                    System.IO.FileInfo fi = new System.IO.FileInfo(file);
                    listPath.Add(fi.Name);
                    listPath.Add(fi.CreationTime.ToString("yyMMddHHmmss"));
                    listPath.Add("1"); // File
                    listPath.Add("0");
                }
                catch (System.IO.FileNotFoundException e)
                {
                    // If file was deleted by a separate application
                    //  or thread since the call to TraverseTree()
                    // then just continue.
                    //Console.WriteLine(e.Message);
                    continue;
                }
            }
        }

        private void CreateFolder()
        {
            try
            {
                byte[] buffer = new byte[1500];

                /* tạo object truyền nhận lệnh với client */
                NetworkStream sr = m_Client.GetStream();
                NetworkStream sw = m_Client.GetStream();
                /* chờ nhận thông tin file */
                int iBytesread = sr.Read(buffer, 0, buffer.Length);

                /* lấy 4 bytes dầu tiên là kích thước của folder path */
                int FolderNameLen = BitConverter.ToInt32(buffer, 0);

                /* lấy folder path bắt đầu từ vị trí thứ 4 */
                string sPathFull = Encoding.UTF8.GetString(buffer, 4, FolderNameLen);

                m_FolderPath = System.IO.Path.Combine(m_FolderPath, sPathFull);
                /* nếu Folder không tồn tại */
                int iCommandIndex = 0;
                if (Directory.Exists(m_FolderPath))
                {
                    /* gửi lệnh folder đã tồn tại về client */
                    iCommandIndex = 101; // thư mục tồn tại
                    byte[] commandByte = BitConverter.GetBytes(iCommandIndex);
                    sw.Write(commandByte, 0, commandByte.Length);
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(m_FolderPath);
                        iCommandIndex = 100; // Successfull
                        byte[] commandByte = BitConverter.GetBytes(iCommandIndex);

                        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(m_FolderPath);
                        string sCreationTime = dir.CreationTime.ToString("yyMMddHHmmss");
                        byte[] LenCreationTimeByte = BitConverter.GetBytes(sCreationTime.Length);
                        byte[] CreationTimeByte = Encoding.UTF8.GetBytes(sCreationTime);

                        byte[] byteResult = new byte[commandByte.Length + LenCreationTimeByte.Length + CreationTimeByte.Length];
                        commandByte.CopyTo(byteResult, 0);
                        LenCreationTimeByte.CopyTo(byteResult, commandByte.Length);
                        CreationTimeByte.CopyTo(byteResult, LenCreationTimeByte.Length);

                        sw.Write(byteResult, 0, byteResult.Length);
                        //sw.Write(commandByte, 0, commandByte.Length);
                    }
                    catch
                    {
                        iCommandIndex = 102; // Lỗi trong quá trình tạo thư mục
                        byte[] commandByteNotFound = BitConverter.GetBytes(iCommandIndex);
                        sw.Write(commandByteNotFound, 0, commandByteNotFound.Length);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                //System.Diagnostics.Debug.WriteLine(iNumber);
                if (m_Client != null) m_Client.Close();
                m_Client = null;
                if (m_Thread != null) m_Thread.Abort();
                m_Thread = null;
            }

        }
        #endregion

        #region Public Methods
        public void StartThread()
        {
            m_Thread.Start();
        }

        public void StopThread()
        {
            m_Thread.Abort();
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            // Check to see if Dispose has already been called.
            if (!this.disposeCalled)
            {
                Dispose(true);
                // This object will be cleaned up by the Dispose method.
                // Therefore, you should call GC.SupressFinalize to
                // take this object off the finalization queue
                // and prevent finalization code for this object
                // from executing a second time.
                GC.SuppressFinalize(this);
            }
        }

        private void Dispose(bool disposing)
        {
            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.

            }

            // dispose unmanaged resources here.
            // If disposing is false,
            // only the following code is executed.
            if (m_Client != null) m_Client.Close();
            m_Client = null;
            if (m_Thread != null) m_Thread.Abort();
            m_Thread = null;

            disposeCalled = true;
        }

        ~MKTranferFile()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
