using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace MKServer
{
    public class AsynchronousSocketListener
    {
        Socket listener;
        // Thread signal.
        public ManualResetEvent allDone = new ManualResetEvent(false);
        public string ServerIpAddress { get; set; }
        public string ServerPath { get; set; }
        public bool StartServer { get; set; }
        public int ServerPort { get; set; }
        const int BufferSize = 1500;
        const string commandAgree = "ACK";
        const string commandOK = "OK";
        public AsynchronousSocketListener()
        {
        }

        #region enum
        //enum FtpStatusResponse
        // {
        //     AccountNeeded = 532,
        //     ActionAbortedLocalProcessingError = 451,
        //     ActionAbortedUnknownPageType = 551,
        //     ActionNotTakenFilenameNotAllowed = 553,
        //     ActionNotTakenFileUnavailable = 550,
        //     ActionNotTakenFileUnavailableOrBusy = 450,
        //     ActionNotTakenInsufficientSpace = 452,
        //     ArgumentSyntaxError = 501,
        //     BadCommandSequence = 503,
        //     CantOpenData = 425,
        //     ClosingControl = 221,
        //     ClosingData = 226,
        //     CommandExtraneous = 202,
        //     CommandNotImplemented = 502,
        //     CommandOK = 200,
        //     CommandSyntaxError = 500,
        //     ConnectionClosed = 426,
        //     DataAlreadyOpen = 125,
        //     DirectoryStatus = 212,
        //     EnteringPassive = 227,
        //     FileActionAborted = 552,
        //     FileActionOK = 250,
        //     FileCommandPending = 350,
        //     FileStatus = 213,
        //     LoggedInProceed = 230,
        //     NeedLoginAccount = 332,
        //     NotLoggedIn = 530,
        //     OpeningData = 150,
        //     PathnameCreated = 257,
        //     RestartMarker = 110,
        //     SendPasswordCommand = 331,
        //     SendUserCommand = 220,
        //     ServerWantsSecureSession = 234,
        //     ServiceNotAvailable = 421,
        //     ServiceTemporarilyNotAvailable = 120,
        //     SystemType = 215,
        //     Undefined = 0
        // }

        enum DocumentStatusCode
        {
            Successful,
            NotFound,
            FolderEmpty,
            Error
        }

        enum Function
        {
            DownloadFile,
            UploadFile,
            DeleteFile,
            CreateFolder,
            FindPath
        }

        enum Step
        {
            Initial,
            Begin,
            Working,
            Completed
        }
        #endregion




        public void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[BufferSize];

            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            IPAddress ipAddress = IPAddress.Parse(ServerIpAddress);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, ServerPort);

            // Create a TCP/IP socket.
            listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    //Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                                       new AsyncCallback(AcceptCallback),
                                       listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (ObjectDisposedException)
            {
                //Console.WriteLine("Listener closed.");
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }

            //Console.WriteLine("\nPress ENTER to continue...");
            //Console.Read();

        }

        public void AcceptCallback(IAsyncResult ar)
        {
            try
            { // Signal the main thread to continue.
                allDone.Set();

                // Get the socket that handles the client request.
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = handler;
                state.step = (int)Step.Initial;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            { }
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                if (state.step == (int)Step.Initial)
                {
                    // There  might be more data, so store the data received so far.
                    //state.sb.Append(Encoding.UTF8.GetString(
                    //    state.buffer, 0, bytesRead));

                    // Check for end-of-file tag. If it is not there, read 
                    // more data.
                    content = Encoding.UTF8.GetString(state.buffer, 0, bytesRead);
                    state.step = (int)Step.Begin;
                    /* phân tích lệnh */
                    switch (content)
                    {
                        case "GET": // download file

                            break;
                        case "PUT": // upload file

                            break;

                        case "DEL": // Delete file
                            break;

                        case "FIN": // duyệt cây thư mục
                            state.function = (int)Function.FindPath;
                            int bytesCommandFind = commandAgree.Length;
                            byte[] commandByteFind = Encoding.UTF8.GetBytes(commandAgree);
                            ///* gửi lệnh đồng ý về client */
                            // Begin sending the data to the remote device.  
                            handler.BeginSend(commandByteFind, 0, bytesCommandFind, 0, new AsyncCallback(SendCallback), state);

                            //Waiting client send folder path 
                            handler.BeginReceive(state.buffer, 0, state.buffer.Length, 0,
                            new AsyncCallback(ReadCallback), state);
                            break;

                        case "CRD": // Tạo Thư mục
                            break;


                    }
                }
                else if (state.step == (int)Step.Begin)
                {
                    state.step = (int)Step.Working;
                    switch (state.function)
                    {
                        case (int)Function.DownloadFile: // download file

                            break;
                        case (int)Function.UploadFile: // upload file

                            break;

                        case (int)Function.DeleteFile: // Delete file
                            break;

                        case (int)Function.FindPath: // duyệt cây thư mục
                            FindPaths(state, bytesRead, handler);
                            break;

                        case (int)Function.CreateFolder: // Tạo Thư mục
                            break;


                    }
                }
                else if (state.step == (int)Step.Working)
                {
                    switch (state.function)
                    {
                        case (int)Function.DownloadFile: // download file

                            break;
                        case (int)Function.UploadFile: // upload file

                            break;

                        case (int)Function.DeleteFile: // Delete file
                            break;

                        case (int)Function.FindPath: // duyệt cây thư mục
                            ReturnListPath(state, bytesRead, handler);
                            break;

                        case (int)Function.CreateFolder: // Tạo Thư mục
                            break;
                    }

                }
                else if (state.step == (int)Step.Completed)
                {
                }
            }
        }

        #region Send
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                if (state.step == (int)Step.Completed)
                {
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion

        #region FindPath
        void FindPaths(StateObject state, int bytesRead, Socket handler)
        {
            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                int FolderNameLen = BitConverter.ToInt32(state.buffer, 0);
                /* lấy file or folder path bắt đầu từ vị trí thứ 4 */
                string sRelativePath = Encoding.UTF8.GetString(state.buffer, 4, FolderNameLen);
                string sFullPath = System.IO.Path.Combine(ServerPath, sRelativePath);
                if (!Directory.Exists(sFullPath))
                {
                    state.step = (int)Step.Completed;
                    /* gửi lệnh file không tồn tại về client */
                    byte[] commandByte = BitConverter.GetBytes((int)DocumentStatusCode.NotFound);
                    handler.BeginSend(commandByte, 0, commandByte.Length, 0,
                    new AsyncCallback(SendCallback), state);
                }
                else
                {
                    try
                    {
                        // Tìm danh sach folder và file trong Path
                        List<string> listPath = new List<string>();
                        byte[] buffer = new byte[BufferSize];
                        TraverseTree(sFullPath, listPath);

                        if (listPath.Count > 0)
                        {
                            byte[] commandByteOK = BitConverter.GetBytes((int)DocumentStatusCode.Successful);
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

                            // Send len data
                            handler.BeginSend(buffer, 0, commandByteOK.Length + commandByteCount.Length, 0,
                                            new AsyncCallback(SendCallback), state);

                            state.totalLen = totalLen;
                            state.listPath = listPath;
                            //Waiting client send OK commnad
                            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                            new AsyncCallback(ReadCallback), state);
                        }
                        else
                        {
                            //Trường hợp Folder Empty
                            state.step = (int)Step.Completed;
                            /* gửi lệnh lỗi về client */
                            byte[] commandByte = BitConverter.GetBytes((int)DocumentStatusCode.FolderEmpty);
                            handler.BeginSend(commandByte, 0, commandByte.Length, 0,
                            new AsyncCallback(SendCallback), state);
                        }
                    }
                    catch
                    {
                        // Trường hợp bị lỗi
                        state.step = (int)Step.Completed;
                        /* gửi lệnh lỗi về client */
                        byte[] commandByte = BitConverter.GetBytes((int)DocumentStatusCode.Error);
                        handler.BeginSend(commandByte, 0, commandByte.Length, 0,
                        new AsyncCallback(SendCallback), state);
                    }
                }
            }
        }
        void ReturnListPath(StateObject state, int bytesRead, Socket handler)
        {
            if (bytesRead > 0)
            {
                if (Encoding.UTF8.GetString(state.buffer, 0, bytesRead) == "OK")
                {
                    byte[] byteResult = new byte[state.totalLen];
                    long totalLen = 0;

                    for (int i = 0; i < state.listPath.Count; i += 4)
                    {
                        byte[] byteItem = Encoding.UTF8.GetBytes(state.listPath[i]);
                        byte[] byteLenItem = BitConverter.GetBytes(byteItem.Length);
                        byte[] byteCreationTime = Encoding.UTF8.GetBytes(state.listPath[i + 1]);
                        byte[] byteFolderOrFile = Encoding.UTF8.GetBytes(state.listPath[i + 2]);
                        byte[] byteHaveSubFolder = Encoding.UTF8.GetBytes(state.listPath[i + 3]);
                        byteLenItem.CopyTo(byteResult, totalLen);
                        byteItem.CopyTo(byteResult, totalLen + byteLenItem.Length);
                        byteCreationTime.CopyTo(byteResult, totalLen + byteLenItem.Length + byteItem.Length);
                        byteFolderOrFile.CopyTo(byteResult, totalLen + byteLenItem.Length + byteItem.Length + 12);
                        byteHaveSubFolder.CopyTo(byteResult, totalLen + byteLenItem.Length + byteItem.Length + 12 + 1);
                        totalLen += byteLenItem.Length + byteItem.Length + 12 + 1 + 1;
                    }
                    int iBytesread = 0;
                    while (totalLen > 0)
                    {
                        if (totalLen >= BufferSize)
                        {
                            handler.BeginSend(byteResult, iBytesread, BufferSize, 0, new AsyncCallback(SendCallback), state);
                        }
                        else
                        {
                            state.step = (int)Step.Completed;
                            handler.BeginSend(byteResult, iBytesread, (int)totalLen, 0, new AsyncCallback(SendCallback), state);
                        }
                        totalLen -= BufferSize;
                        iBytesread += BufferSize;
                    }
                }
            }
           
        }
        #endregion

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

            string[] files = null;
            try
            {
                files = System.IO.Directory.GetFiles(root);
            }

            catch// (UnauthorizedAccessException e)
            {
                return;
            }

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
