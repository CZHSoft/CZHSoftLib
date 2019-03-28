using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CZHSoft.IOCP
{
    /// <summary>
    /// 守护线程
    /// </summary>
    class DaemonThread : Object
    {
        private Thread m_thread;
        private AsyncSocketServer m_asyncSocketServer;

        public DaemonThread(AsyncSocketServer asyncSocketServer)
        {
            m_asyncSocketServer = asyncSocketServer;
            m_thread = new Thread(DaemonThreadStart);
            m_thread.Start();
        }

        /// <summary>
        /// 监测
        /// </summary>
        public void DaemonThreadStart()
        {
            while (m_thread.IsAlive)
            {
                AsyncSocketUserToken[] userTokenArray = null;
                m_asyncSocketServer.AsyncSocketUserTokenList.CopyList(ref userTokenArray);
                for (int i = 0; i < userTokenArray.Length; i++)
                {
                    if (!m_thread.IsAlive)
                        break;
                    try
                    {
                        if ((DateTime.Now - userTokenArray[i].ActiveDateTime).Milliseconds > m_asyncSocketServer.SocketTimeOutMS) //超时Socket断开
                        {
                            lock (userTokenArray[i])
                            {
                                m_asyncSocketServer.CloseClientSocket(userTokenArray[i]);
                            }
                        }
                    }                    
                    catch (Exception E)
                    {
                        //Program.Logger.ErrorFormat("Daemon thread check timeout socket error, message: {0}", E.Message);
                        //Program.Logger.Error(E.StackTrace);
                    }
                }

                for (int i = 0; i < 60 * 1000 / 10; i++) //每分钟检测一次
                {
                    if (!m_thread.IsAlive)
                        break;
                    Thread.Sleep(10);
                }
            }
        }

        /// <summary>
        /// 监测线程关闭
        /// </summary>
        public void Close()
        {
            m_thread.Abort();
            m_thread.Join();
        }
    
    }
}
