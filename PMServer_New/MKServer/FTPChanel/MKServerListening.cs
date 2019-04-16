/*=================================================================
Project                :  
Package                :  
Module                 :  
Description            :  
Start Date             :  08/07/2017
Finish Date            :  08/07/2017
Last Update            :  
Class Name             :  MKServerListening
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
    public class MKServerListening : IDisposable
    {
        #region Propertiy variables
        private string m_ServerPath;
        private string m_ServerIPAddress;
        private int m_ServerPort;
        private bool m_StartServer;
        #endregion

        #region Local variables
        private Thread m_Thread;
        private TcpListener gFileListener;
        private bool disposeCalled;
        #endregion

        #region Constructors
        public MKServerListening()
        {
            disposeCalled = false;
            m_ServerPort = 8085;
            m_ServerIPAddress = "127.0.0.1";
        }
        #endregion

        #region Properties
        public string ServerPath
        {
            get { return m_ServerPath; }
            set { m_ServerPath = value; }
        }
        
        public string ServerIPAddress
        {
            get { return m_ServerIPAddress; }
            set { m_ServerIPAddress = value; }
        }

        public int ServerPort
        {
            get { return m_ServerPort; }
            set { m_ServerPort = value; }
        }
        #endregion

        #region Private Methods
        //private void ListenToClient()
        //{
        //    IPAddress ip = IPAddress.Any;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(m_ServerIPAddress)) ip = IPAddress.Parse(m_ServerIPAddress);
                //gFileListener = new TcpListener(ip, m_ServerPort);
                //gFileListener.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
        //        gFileListener.Start();
        //        while (m_StartServer)
        //        {
        //            TcpClient client = null;
        //            try
        //            {
        //                client = gFileListener.AcceptTcpClient();
        //                if (client == null) continue;
        //            }
        //            catch
        //            {
        //                continue;
        //            }
        //            if (m_StartServer)
        //            {
        //                MKTranferFile newThread = new MKTranferFile(client);
        //                newThread.FolderPath = m_ServerPath;  // @"D:\\Downloads\\";
        //                newThread.StartThread();
        //            }
        //            else
        //            {
        //                NetworkStream sw = client.GetStream();
        //                string sCommand = "NOTACK";
        //                int bytesCommand = sCommand.Length;
        //                byte[] commandByte = Encoding.UTF8.GetBytes(sCommand);
        //                try { sw.Write(commandByte, 0, bytesCommand); }
        //                catch
        //                {
        //                    sw.Close();
        //                    client.Close();
        //                    break;
        //                }

        //                client.Close();
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    finally
        //    {
        //        StopListening();
        //        //StartListening();
        //    }
        //}


        private void ListenToClient()
        {
        }
        #endregion

        #region Public Methods
        public void StartListening()
        {
            m_StartServer = true;
            m_Thread = new Thread(new ThreadStart(ListenToClient));
            m_Thread.Start();
        }

        public void StopListening()
        {
            m_StartServer = false;
            if (gFileListener != null) gFileListener.Stop();
            gFileListener = null;
            if (m_Thread != null)
            {
                if (m_Thread.IsAlive) m_Thread.Abort();
                m_Thread = null;
            }
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Clean up MANAGED resources here. These are guaranteed to be INvalid if 
                //Dispose gets called by the constructor
            }

            m_StartServer = false;
            if (gFileListener != null) gFileListener.Stop();
            gFileListener = null;
            if (m_Thread != null)
            {
                if (m_Thread.IsAlive) m_Thread.Abort();
                m_Thread = null;
            }
            
            disposeCalled = true;
        }
        
        ~MKServerListening()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
