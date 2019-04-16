using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace MKServer
{
    public class SynchronousSocketListener
    {
        Socket listener;
        public string ServerIpAddress { get; set; }
        public string ServerPath { get; set; }
        public bool StartServer { get; set; }
        public int ServerPort { get; set; }
        const int BufferSize = 1500;
        const string commandAgree = "ACK";
        const string commandOK = "OK";

        //DocumentStatusCode
        const int Successful = 200;
        const int NotFound = 404;
        const int FolderEmpty = 403;
        const int FolderExist = 405;
        const int Error = 400;
        //DocumentStatusCode

        public SynchronousSocketListener()
        {
        }

        public void StartListening()
        {

            // Data buffer for incoming data.
            byte[] bytes = new Byte[BufferSize];

            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            IPAddress ipAddress = IPAddress.Parse(ServerIpAddress);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, ServerPort);

            // Create a TCP/IP socket.  
            listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();

                    Thread threadStartFTP = new Thread(() => StartFTP(handler));
                    threadStartFTP.Start();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }  
  
        }

        private void StartFTP(Socket handler)
        {
            byte[] buffer = new byte[BufferSize];

            /* chờ nhận lệnh từ client */
            int iBytesread = handler.Receive(buffer);

            /* đã nhận được lệnh từ client */
            string sCommandRev = Encoding.UTF8.GetString(buffer, 0, iBytesread);

            /* phân tích lệnh */
            switch (sCommandRev)
            {
                case "GET": // download file
                    byte[] commandByte = Encoding.UTF8.GetBytes(commandAgree);

                    /* gửi lệnh đồng ý về client */
                    handler.Send(commandByte);
                    /* gửi nội dung file về client */
                    SendFile(handler);
                    break;
                case "PUT": // upload file
                    byte[] commandBytePut = Encoding.UTF8.GetBytes(commandAgree);

                    //gửi lệnh đồng ý về client
                    handler.Send(commandBytePut);
                    /* gửi nội dung file về client */
                    ReceiveFile(handler);
                    break;

                case "DEL": // Delete file
                    byte[] commandByteDel = Encoding.UTF8.GetBytes(commandAgree);

                    //gửi lệnh đồng ý về client
                    handler.Send(commandByteDel);
                    DeleteFileOrFolder(handler);
                    break;

                case "FIN": // duyệt cây thư mục
                    byte[] commandByteFind = Encoding.UTF8.GetBytes(commandAgree);

                    /* gửi lệnh đồng ý về client */
                    handler.Send(commandByteFind);
                    FindPaths(handler);
                    break;

                case "CRD": // Tạo Thư mục
                    byte[] commandByteCreate = Encoding.UTF8.GetBytes(commandAgree);

                    //gửi lệnh đồng ý về client
                    handler.Send(commandByteCreate);
                    CreateFolder(handler);
                    break;


            }

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        #region FindPaths Level 1
        private void FindPaths(Socket handler)
        {
            try
            {
                List<string> listPath = null;
                byte[] buffer = new byte[BufferSize];

                /* chờ nhận thông tin file */
                int iBytesread = handler.Receive(buffer);

                /* lấy 4 bytes dầu tiên là kích thước của file or folder path */
                int FolderNameLen = BitConverter.ToInt32(buffer, 0);

                /* lấy file or folder path bắt đầu từ vị trí thứ 4 */
                string sPathRelative = Encoding.UTF8.GetString(buffer, 4, FolderNameLen);

                string pathFull = System.IO.Path.Combine(ServerPath, sPathRelative);
                /* nếu file không tồn tại */
                if (!Directory.Exists(pathFull))
                {
                    /* gửi lệnh file không tồn tại về client */
                    // Folder not exist
                    byte[] commandByte = BitConverter.GetBytes(NotFound);
                    handler.Send(commandByte);
                }
                else
                {
                    try
                    {
                        listPath = new List<string>();
                        TraverseTree(pathFull, listPath);
                        if (listPath.Count > 0)
                        {
                            /* gửi lệnh Found */
                            byte[] commandByteOK = BitConverter.GetBytes(Successful);
                            commandByteOK.CopyTo(buffer, 0);


                            long totalLen = 0;
                            for (int i = 0; i < listPath.Count; i += 4)
                            {
                                byte[] byteItem = Encoding.UTF8.GetBytes(listPath[i]); // mảng byte tên file
                                byte[] byteLenItem = BitConverter.GetBytes(byteItem.Length); // mảng byte chiều dài tên file
                                totalLen += byteLenItem.Length + byteItem.Length + 12 + 1 + 1; // 12 là length của yyMMddHHmmss, 1 là Folder Or file (0 or 1), 1 kiem tra co subfolder hay khong
                            }
                            byte[] commandByteCount = BitConverter.GetBytes(totalLen);
                            commandByteCount.CopyTo(buffer, commandByteOK.Length);

                            //Send data to client
                            handler.Send(buffer.Take(commandByteOK.Length + commandByteCount.Length).ToArray());

                            //Receive data from client
                            iBytesread = handler.Receive(buffer);
                            if (Encoding.UTF8.GetString(buffer, 0, iBytesread) == commandOK)
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

                                iBytesread = 0;
                                while (totalLen > 0)
                                {
                                    if (totalLen >= BufferSize)
                                    {
                                        handler.Send(byteResult.Skip(iBytesread).Take(BufferSize).ToArray());
                                    }
                                    else
                                    {
                                        handler.Send(byteResult.Skip(iBytesread).Take((int)totalLen).ToArray());
                                    }
                                    totalLen -= 1500;
                                    iBytesread += 1500;
                                }
                            }
                        }
                        else
                        {
                            /* gửi lệnh Thư mục rỗng */
                            byte[] commandByteOK = BitConverter.GetBytes(FolderEmpty);
                            handler.Send(commandByteOK);
                        }
                    }
                    catch
                    {
                        // Error
                        byte[] commandByteNotFound = BitConverter.GetBytes(Error);
                        handler.Send(commandByteNotFound);
                    }
                }
            }
            catch
            {
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
        #endregion

        #region Send file
        private void SendFile(Socket handler)
        {
            StreamReader srReadFile = null;
            try
            {
                byte[] buffer = new byte[BufferSize];

                /* chờ nhận thông tin file */
                int iBytesread = handler.Receive(buffer);

                /* lấy 4 bytes dầu tiên là kích thước của file path */
                int fileNameLen = BitConverter.ToInt32(buffer, 0);

                /* lấy file path bắt đầu từ vị trí thứ 4 */
                string sFileRelativePath = Encoding.UTF8.GetString(buffer, 4, fileNameLen);
                string sPathFull = System.IO.Path.Combine(ServerPath, sFileRelativePath);

                /* nếu file không tồn tại */
                if (!System.IO.File.Exists(sPathFull))
                {
                    /* gửi lệnh file không tồn tại về client */
                    byte[] commandByte = BitConverter.GetBytes(NotFound);
                    handler.Send(commandByte);
                    return;
                }

                /* nếu file tồn tại, gửi lệnh đồng ý về client */
                byte[] commandByteOK = BitConverter.GetBytes(Successful);
                commandByteOK.CopyTo(buffer, 0);

                /* mở file */
                srReadFile = new StreamReader(sPathFull);

                long lFileSize = srReadFile.BaseStream.Length;
                byte[] fileLen = BitConverter.GetBytes(lFileSize);
                fileLen.CopyTo(buffer, commandByteOK.Length);

                /* truyền kích thước file về client */
                handler.Send(buffer.Take(commandByteOK.Length + fileLen.Length).ToArray());

                /* chờ nhận thông tin client */
                iBytesread = handler.Receive(buffer);

                int iCommandIndex = BitConverter.ToInt32(buffer, 0);
                if (iCommandIndex != Successful) // not OK
                {
                    return;
                }

                /* truyền nội dung file */
                long bytesSent = 0;
                int bytesRead = 0;
                while (bytesSent < lFileSize)
                {
                    bytesRead = srReadFile.BaseStream.Read(buffer, 0, BufferSize);
                    handler.Send(buffer.Take(bytesRead).ToArray());
                    bytesSent += bytesRead;
                }
            }
            catch { }
            finally
            {
                //System.Diagnostics.Debug.WriteLine(iNumber);
                if (srReadFile != null) srReadFile.Close();
                srReadFile = null;
            }

        }
        #endregion

        #region Delete File Or Folder
        private void DeleteFileOrFolder(Socket handler)
        {
            try
            {
                byte[] buffer = new byte[BufferSize];

                /* chờ nhận thông tin file */
                //int iBytesread = sr.Read(buffer, 0, buffer.Length);
                int iBytesread = handler.Receive(buffer);

                /* lấy 4 bytes dầu tiên là kích thước của file or folder path */
                int fileOrFolderNameLen = BitConverter.ToInt32(buffer, 0);

                /* lấy file or folder path bắt đầu từ vị trí thứ 4 */
                string sPathRelative = Encoding.UTF8.GetString(buffer, 4, fileOrFolderNameLen);

                string sFullPath = System.IO.Path.Combine(ServerPath, sPathRelative);
                /* nếu file hoặc folder không tồn tại */
                if (!System.IO.File.Exists(sFullPath) && !Directory.Exists(sFullPath))
                {
                    /* gửi lệnh file or folder không tồn tại về client */
                    byte[] commandByte = BitConverter.GetBytes(NotFound);
                    //sw.Write(commandByte, 0, commandByte.Length);
                    handler.Send(commandByte);
                }
                else
                {
                    try
                    {
                        if (System.IO.File.Exists(sFullPath)) System.IO.File.Delete(sFullPath);
                        else Directory.Delete(sFullPath, true);
                        /* gửi lệnh đã xóa file thành công */
                        byte[] commandByteOK = BitConverter.GetBytes(Successful);
                        //sw.Write(commandByteOK, 0, commandByteOK.Length);
                        handler.Send(commandByteOK);
                    }
                    catch
                    {
                        // Error
                        byte[] commandByteNotDelete = BitConverter.GetBytes(Error);
                        //sw.Write(commandByteNotDelete, 0, commandByteNotDelete.Length);
                        handler.Send(commandByteNotDelete);

                    }
                }
            }
            catch
            {
            }

        }
        #endregion

        #region Create Folder
        private void CreateFolder(Socket handler)
        {
            try
            {
                byte[] buffer = new byte[BufferSize];

                /* chờ nhận thông tin file */
                //int iBytesread = sr.Read(buffer, 0, buffer.Length);
                int iBytesread = handler.Receive(buffer);

                /* lấy 4 bytes dầu tiên là kích thước của folder path */
                int FolderNameLen = BitConverter.ToInt32(buffer, 0);

                /* lấy folder path bắt đầu từ vị trí thứ 4 */
                string sRelativePath = Encoding.UTF8.GetString(buffer, 4, FolderNameLen);

                string sFullFolderPath = System.IO.Path.Combine(ServerPath, sRelativePath);

                /* nếu Folder tồn tại */
                if (Directory.Exists(sFullFolderPath))
                {
                    /* gửi lệnh folder đã tồn tại về client */
                    byte[] commandByte = BitConverter.GetBytes(FolderExist);
                    //sw.Write(commandByte, 0, commandByte.Length);
                    handler.Send(commandByte);
                }
                else
                {
                    try
                    {
                        Directory.CreateDirectory(sFullFolderPath);
                        // Successfull
                        byte[] commandByte = BitConverter.GetBytes(Successful);

                        System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(sFullFolderPath);
                        string sCreationTime = dir.CreationTime.ToString("yyMMddHHmmss");
                        byte[] LenCreationTimeByte = BitConverter.GetBytes(sCreationTime.Length);
                        byte[] CreationTimeByte = Encoding.UTF8.GetBytes(sCreationTime);

                        byte[] byteResult = new byte[commandByte.Length + LenCreationTimeByte.Length + CreationTimeByte.Length];
                        commandByte.CopyTo(byteResult, 0);
                        LenCreationTimeByte.CopyTo(byteResult, commandByte.Length);
                        CreationTimeByte.CopyTo(byteResult, LenCreationTimeByte.Length);

                        //sw.Write(byteResult, 0, byteResult.Length);
                        handler.Send(byteResult);
                    }
                    catch
                    {
                        // Lỗi trong quá trình tạo thư mục
                        byte[] commandByteError = BitConverter.GetBytes(Error);
                        //sw.Write(commandByteNotFound, 0, commandByteNotFound.Length);
                        handler.Send(commandByteError);
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region Upload File
        private void ReceiveFile(Socket handler)
        {
            string sError = string.Empty;
            try
            {
                byte[] buffer = new byte[BufferSize];


                /* nhận kích thước file, kích thước tên file và tên file của khối truyền đầu tiên */
                //int iBytesread = sr.Read(buffer, 0, buffer.Length);
                int iBytesread = handler.Receive(buffer);

                /* 8 bytes nhận đầu tiên trong buffer là kích thước của file */
                long lFileSize = BitConverter.ToInt64(buffer, 0);

                /* kế tiếp là chiều dài của tên file */
                int fileNameLen = BitConverter.ToInt32(buffer, 8);

                /* sau đó là vị trí của fileName (8 + 4 = 12) */
                string sRelativeFilePath = Encoding.UTF8.GetString(buffer, 12, fileNameLen);

                string sFilePathFull = System.IO.Path.Combine(ServerPath, sRelativeFilePath);
                
                // File Name
                string sFileName = System.IO.Path.GetFileName(sFilePathFull);
                //Folder Path contain file name
                string sAbsoluteFolderPath = System.IO.Path.GetDirectoryName(sFilePathFull);
                // if folder not exist then create new foler
                if (!System.IO.Directory.Exists(sAbsoluteFolderPath)) System.IO.Directory.CreateDirectory(sAbsoluteFolderPath);

                int iNumeric = 0;
                string extension;
                string sCurFileName = sFileName;
                while (System.IO.File.Exists(sFilePathFull))
                {
                    iNumeric++;
                    extension = Path.GetExtension(sFileName);
                    sCurFileName = sFileName.Replace(extension, "(" + iNumeric.ToString() + ")" + extension);
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
                    //bytesread = sr.Read(buffer, 0, buffer.Length);
                    bytesread = handler.Receive(buffer);
                    lbytesRevceive += bytesread;
                    if (lbytesRevceive > lFileSize) break;
                    writer.BaseStream.Write(buffer, 0, bytesread);
                }

                byte[] commandByte = BitConverter.GetBytes(Successful);
                byte[] pathByte = Encoding.UTF8.GetBytes(sCurFileName/*sFilePathFull*/);
                byte[] pathLen = BitConverter.GetBytes(pathByte.Length);

                commandByte.CopyTo(buffer, 0);
                pathLen.CopyTo(buffer, commandByte.Length);
                pathByte.CopyTo(buffer, commandByte.Length + pathLen.Length);
                //sw.Write(buffer, 0, commandByte.Length + commandByte.Length + pathByte.Length);
                handler.Send(buffer.Take(commandByte.Length + commandByte.Length + pathByte.Length).ToArray());
                writer.Close();
            }
            catch { }
        }
        #endregion

        public void StopListening() // Stop Listening
        {
            Socket exListener = Interlocked.Exchange(ref listener, null);
            if (exListener != null)
            {
                exListener.Close();
            }
        }
    }
}
