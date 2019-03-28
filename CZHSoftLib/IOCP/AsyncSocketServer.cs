using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CZHSoft.IOCP
{
    public class AsyncSocketServer
    {
        private Socket listenSocket;
        
        private int m_numConnections; //最大支持连接个数
        private int m_receiveBufferSize; //每个连接接收缓存大小
        private Semaphore m_maxNumberAcceptedClients; //限制访问接收连接的线程数，用来控制最大并发数

        private int m_socketTimeOutMS; //Socket最大超时时间，单位为MS
        public int SocketTimeOutMS { get { return m_socketTimeOutMS; } set { m_socketTimeOutMS = value; } }
               
        private AsyncSocketUserTokenPool m_asyncSocketUserTokenPool;
        private AsyncSocketUserTokenList m_asyncSocketUserTokenList;
        public AsyncSocketUserTokenList AsyncSocketUserTokenList { get { return m_asyncSocketUserTokenList; } }

        //private LogOutputSocketProtocolMgr m_logOutputSocketProtocolMgr;
        //public LogOutputSocketProtocolMgr LogOutputSocketProtocolMgr { get { return m_logOutputSocketProtocolMgr; } }

        //private UploadSocketProtocolMgr m_uploadSocketProtocolMgr;
        //public UploadSocketProtocolMgr UploadSocketProtocolMgr { get { return m_uploadSocketProtocolMgr; } }

        //private DownloadSocketProtocolMgr m_downloadSocketProtocolMgr;
        //public DownloadSocketProtocolMgr DownloadSocketProtocolMgr { get { return m_downloadSocketProtocolMgr; } }

        private DaemonThread m_daemonThread;

        public delegate void ReturnMSGDelegate(string msg);
        public event ReturnMSGDelegate OnReturnMSG;

        /// <summary>
        /// AsyncSocketServer构造
        /// </summary>
        /// <param name="numConnections"></param>
        public AsyncSocketServer(int numConnections)
        {
            m_numConnections = numConnections;
            //m_receiveBufferSize = ProtocolConst.ReceiveBufferSize;

            m_asyncSocketUserTokenPool = new AsyncSocketUserTokenPool(numConnections);
            m_asyncSocketUserTokenList = new AsyncSocketUserTokenList();
            m_maxNumberAcceptedClients = new Semaphore(numConnections, numConnections);

            //m_logOutputSocketProtocolMgr = new LogOutputSocketProtocolMgr();
            //m_uploadSocketProtocolMgr = new UploadSocketProtocolMgr();
            //m_downloadSocketProtocolMgr = new DownloadSocketProtocolMgr();
        }

        /// <summary>
        /// 初始化资源
        /// </summary>
        public void InitUserToken()
        {
            AsyncSocketUserToken userToken;
            for (int i = 0; i < m_numConnections; i++) //按照连接数建立读写对象
            {
                userToken = new AsyncSocketUserToken(m_receiveBufferSize);
                userToken.ReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                userToken.SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                m_asyncSocketUserTokenPool.Push(userToken);
            }
        }

        /// <summary>
        /// 绑定并监听
        /// </summary>
        /// <param name="localEndPoint"></param>
        public void Start(IPEndPoint localEndPoint)
        {
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);
            listenSocket.Listen(m_numConnections);

            if (OnReturnMSG != null)
            {
                OnReturnMSG(string.Format("Start listen socket {0} success", localEndPoint.ToString()));
            }

            //for (int i = 0; i < 64; i++) //不能循环投递多次AcceptAsync，会造成只接收8000连接后不接收连接了

            StartAccept(null);
            m_daemonThread = new DaemonThread(this);
        }

        /// <summary>
        /// 接入初期处理
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        public void StartAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (acceptEventArgs == null)
            {
                acceptEventArgs = new SocketAsyncEventArgs();
                acceptEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);
            }
            else
            {
                acceptEventArgs.AcceptSocket = null; //释放上次绑定的Socket，等待下一个Socket连接
            }

            m_maxNumberAcceptedClients.WaitOne(); //获取信号量
            bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArgs);
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptEventArgs);
            }
        }

        void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs acceptEventArgs)
        {
            try
            {
                ProcessAccept(acceptEventArgs);
            }
            catch (Exception E)
            {
                if (OnReturnMSG != null)
                {
                    OnReturnMSG(string.Format("Accept client {0} error, message: {1}", acceptEventArgs.AcceptSocket, E.Message));
                }
            }            
        }

        /// <summary>
        /// 接入后期处理，子连接作接受等待，并作下次连接等待
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            if (OnReturnMSG != null)
            {
                OnReturnMSG(string.Format("Client connection accepted. Local Address: {0}, Remote Address: {1}", acceptEventArgs.AcceptSocket.LocalEndPoint, acceptEventArgs.AcceptSocket.RemoteEndPoint));
            }

            AsyncSocketUserToken userToken = m_asyncSocketUserTokenPool.Pop();
            m_asyncSocketUserTokenList.Add(userToken); //添加到正在连接列表
            userToken.ConnectSocket = acceptEventArgs.AcceptSocket;
            userToken.ConnectDateTime = DateTime.Now;

            try
            {
                bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs); //投递接收请求
                if (!willRaiseEvent)
                {
                    lock (userToken)
                    {
                        ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }                    
            }
            catch (Exception E)
            {
                if (OnReturnMSG != null)
                {
                    OnReturnMSG(string.Format("Accept client {0} error, message: {1}", userToken.ConnectSocket, E.Message));
                }
            }            

            StartAccept(acceptEventArgs); //把当前异步事件释放，等待下次连接
        }

        void IO_Completed(object sender, SocketAsyncEventArgs asyncEventArgs)
        {
            AsyncSocketUserToken userToken = asyncEventArgs.UserToken as AsyncSocketUserToken;
            userToken.ActiveDateTime = DateTime.Now;
            try
            {                
                lock (userToken)
                {
                    if (asyncEventArgs.LastOperation == SocketAsyncOperation.Receive)
                        ProcessReceive(asyncEventArgs);
                    else if (asyncEventArgs.LastOperation == SocketAsyncOperation.Send)
                        ProcessSend(asyncEventArgs);
                    else
                        throw new ArgumentException("The last operation completed on the socket was not a receive or send");
                }   
            }
            catch (Exception E)
            {
                if (OnReturnMSG != null)
                {
                    OnReturnMSG(string.Format("IO_Completed {0} error, message: {1}", userToken.ConnectSocket, E.Message));
                }
            }                     
        }

        /// <summary>
        /// 接收处理
        /// </summary>
        /// <param name="receiveEventArgs"></param>
        private void ProcessReceive(SocketAsyncEventArgs receiveEventArgs)
        {
            AsyncSocketUserToken userToken = receiveEventArgs.UserToken as AsyncSocketUserToken;
            if (userToken.ConnectSocket == null)
                return;
            userToken.ActiveDateTime = DateTime.Now;
            if (userToken.ReceiveEventArgs.BytesTransferred > 0 && userToken.ReceiveEventArgs.SocketError == SocketError.Success)
            {
                int offset = userToken.ReceiveEventArgs.Offset;
                int count = userToken.ReceiveEventArgs.BytesTransferred;
                if ((userToken.AsyncSocketInvokeElement == null) & (userToken.ConnectSocket != null)) //存在Socket对象，并且没有绑定协议对象，则进行协议对象绑定
                {
                    BuildingSocketInvokeElement(userToken);
                    offset = offset + 1;
                    count = count - 1;
                }
                if (userToken.AsyncSocketInvokeElement == null) //如果没有解析对象，提示非法连接并关闭连接
                {
                    if (OnReturnMSG != null)
                    {
                        OnReturnMSG(string.Format("Illegal client connection. Local Address: {0}, Remote Address: {1}", 
                            userToken.ConnectSocket.LocalEndPoint, userToken.ConnectSocket.RemoteEndPoint));
                    }

                    CloseClientSocket(userToken);
                }
                else
                {
                    if (count > 0) //处理接收数据
                    {
                        if (!userToken.AsyncSocketInvokeElement.ProcessReceive(userToken.ReceiveEventArgs.Buffer, offset, count))
                        { //如果处理数据返回失败，则断开连接
                            CloseClientSocket(userToken);
                        }
                        else //否则投递下次介绍数据请求
                        {
                            bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs); //投递接收请求
                            if (!willRaiseEvent)
                                ProcessReceive(userToken.ReceiveEventArgs);
                        }
                    }
                    else
                    {
                        bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs); //投递接收请求
                        if (!willRaiseEvent)
                            ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }
            }
            else//接收错误
            {
                CloseClientSocket(userToken);
            }
        }

        private void BuildingSocketInvokeElement(AsyncSocketUserToken userToken)
        {
            //byte flag = userToken.ReceiveEventArgs.Buffer[userToken.ReceiveEventArgs.Offset];
            //if (flag == (byte)ProtocolFlag.Upload)
            //    userToken.AsyncSocketInvokeElement = new UploadSocketProtocol(this, userToken);
            //else if (flag == (byte)ProtocolFlag.Download)
            //    userToken.AsyncSocketInvokeElement = new DownloadSocketProtocol(this, userToken);
            //else if (flag == (byte)ProtocolFlag.RemoteStream)
            //    userToken.AsyncSocketInvokeElement = new RemoteStreamSocketProtocol(this, userToken);
            //else if (flag == (byte)ProtocolFlag.Throughput)
            //    userToken.AsyncSocketInvokeElement = new ThroughputSocketProtocol(this, userToken);
            //else if (flag == (byte)ProtocolFlag.Control)
            //    userToken.AsyncSocketInvokeElement = new ControlSocketProtocol(this, userToken);
            //else if (flag == (byte)ProtocolFlag.LogOutput)
            //    userToken.AsyncSocketInvokeElement = new LogOutputSocketProtocol(this, userToken);
            //if (userToken.AsyncSocketInvokeElement != null)
            //{
            //    Program.Logger.InfoFormat("Building socket invoke element {0}.Local Address: {1}, Remote Address: {2}",
            //        userToken.AsyncSocketInvokeElement, userToken.ConnectSocket.LocalEndPoint, userToken.ConnectSocket.RemoteEndPoint);
            //} 
        }

        private bool ProcessSend(SocketAsyncEventArgs sendEventArgs)
        {
            AsyncSocketUserToken userToken = sendEventArgs.UserToken as AsyncSocketUserToken;
            if (userToken.AsyncSocketInvokeElement == null)
                return false;
            userToken.ActiveDateTime = DateTime.Now;
            if (sendEventArgs.SocketError == SocketError.Success)
                return userToken.AsyncSocketInvokeElement.SendCompleted(); //调用子类回调函数
            else
            {
                CloseClientSocket(userToken);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectSocket"></param>
        /// <param name="sendEventArgs"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool SendAsyncEvent(Socket connectSocket, SocketAsyncEventArgs sendEventArgs, byte[] buffer, int offset, int count)
        {
            if (connectSocket == null)
                return false;
            sendEventArgs.SetBuffer(buffer, offset, count);
            bool willRaiseEvent = connectSocket.SendAsync(sendEventArgs);
            if (!willRaiseEvent)
            {
                return ProcessSend(sendEventArgs);
            }
            else
                return true;
        }

        /// <summary>
        /// 断开子连接
        /// </summary>
        /// <param name="userToken"></param>
        public void CloseClientSocket(AsyncSocketUserToken userToken)
        {
            if (userToken.ConnectSocket == null)
                return;
            string socketInfo = string.Format("Local Address: {0} Remote Address: {1}", userToken.ConnectSocket.LocalEndPoint,
                userToken.ConnectSocket.RemoteEndPoint);

            if (OnReturnMSG != null)
            {
                OnReturnMSG(string.Format("Client connection disconnected. {0}",socketInfo));
            }

            try
            {
                userToken.ConnectSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception E) 
            {
                if (OnReturnMSG != null)
                {
                    OnReturnMSG(string.Format("CloseClientSocket Disconnect client {0} error, message: {1}", socketInfo, E.Message));
                }
            }

            userToken.ConnectSocket.Close();
            userToken.ConnectSocket = null; //释放引用，并清理缓存，包括释放协议对象等资源

            m_maxNumberAcceptedClients.Release();
            m_asyncSocketUserTokenPool.Push(userToken);
            m_asyncSocketUserTokenList.Remove(userToken);
        }
    }
}
