using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace TCPServer
{
    class ClientThread
    {
        #region Propertiy variables
        private Thread m_Thread;
        private TcpClient m_Client;

        private string m_FileName;
        private string m_Path;
        #endregion

        #region Local variables
        private bool disposeCalled = false;
        #endregion

        #region Constructors and Disposable
        public ClientThread(TcpClient pClient)
        {
            m_Path = @"D:\\Downloads\\";
            m_Client = pClient;
            m_Thread = new Thread(new ThreadStart(ReceiveFile));
        }

        public void Dispose()
        {
            // Check to see if Dispose has already been called.
            if (!this.disposeCalled)
            {
                Dispose(true);
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
            if (m_Thread != null) m_Thread.Abort();
            m_Thread = null;
            if (m_Client != null) m_Client.Close();
            m_Client = null;

            disposeCalled = true;
        }

        ~ClientThread()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion

        #region Properties
        public string Path
        {
            get { return m_Path; }
            set { m_Path = value; }
        }
        #endregion

        #region Private Methods
        private void ReceiveFile()
        {
            byte[] buffer = new byte[1500];
            int bytesread = 1;

            NetworkStream sr = m_Client.GetStream();
            bytesread = sr.Read(buffer, 0, buffer.Length);
            int fileNameLen = BitConverter.ToInt32(buffer, 0);
            m_FileName = Encoding.UTF8.GetString(buffer, 4, fileNameLen);

            StreamWriter writer = new StreamWriter(m_Path + m_FileName);

            bytesread = 1;
            while (bytesread > 0)
            {
                bytesread = sr.Read(buffer, 0, buffer.Length);
                if (bytesread == 0) break;
                writer.BaseStream.Write(buffer, 0, buffer.Length);
            }

            writer.Close();
            if (m_Client != null) m_Client.Close();
            m_Client = null;
            if (m_Thread != null) m_Thread.Abort();
            m_Thread = null;
            
        }
        #endregion

        #region Public Methods
        public void StartThread()
        {
            m_Thread.Start();
        }

        public void StopThread()
        {
            m_Thread.Start();
        }
        #endregion

    }
}
