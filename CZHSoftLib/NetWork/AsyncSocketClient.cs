using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
//using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CZHSoft.NetWork
{
    public class AsyncSocketClient
    {
        private TcpClient clientSocket;
        private ManualResetEvent allDone = new ManualResetEvent(false);
        private NetworkStream networkStream;
        private bool isListen = false;

        public delegate void SendStatusMSG(string strMsg);
        public event SendStatusMSG OnSendStatusMSG;

        public delegate void SendRecMSG(byte[] byteMSG);
        public event SendRecMSG OnSendRecMSG;

        public delegate void DoConnect(string ip);
        public event DoConnect OnDoConnect;

        public delegate void DoDisconnect(string msg);
        public event DoDisconnect OnDoDisconnect;

        public bool SocketConnect(string ip, int port)
        {
            clientSocket = new TcpClient(AddressFamily.InterNetwork);

            IPAddress[] IP = Dns.GetHostAddresses(ip);

            AsyncCallback connectCallBack = new AsyncCallback(ConnectCallBack);
            allDone.Reset();

            //Console.WriteLine(string.Format("buf size:{0}", clientSocket.SendBufferSize));

            clientSocket.BeginConnect(IP[0], port, connectCallBack, clientSocket);

            if (OnSendStatusMSG != null)
            {
                OnSendStatusMSG(string.Format("开始与服务器[{0}]连接...", clientSocket.Client.LocalEndPoint));
            }

            allDone.WaitOne();

            return true;
        }

        public void SocketDisconnect()
        {
            try
            {
                if (isListen)
                {
                    //3.5
                    clientSocket.Client.Disconnect(false);
                    //4.0
                    //clientSocket.Client.Dispose();

                    isListen = false;

                    if (OnDoDisconnect != null)
                    {
                        OnDoDisconnect("监听已经停止...");
                    }


                }
            }
            catch
            {

            }
            finally
            {
                clientSocket = null;
            }
        }

        private void ConnectCallBack(IAsyncResult iar)
        {
            allDone.Set();

            try
            {
                clientSocket = (TcpClient)iar.AsyncState;
                clientSocket.EndConnect(iar);

                if (OnSendStatusMSG != null)
                {
                    OnSendStatusMSG(string.Format("与服务器[{0}]连接成功...", clientSocket.Client.LocalEndPoint));
                }

                networkStream = clientSocket.GetStream();
                DataRead dataRead = new DataRead(networkStream, clientSocket.ReceiveBufferSize);
                networkStream.BeginRead(dataRead.msg, 0, dataRead.msg.Length, ReadCallBack, dataRead);

                isListen = true;

                if (OnDoConnect != null)
                {
                    OnDoConnect(clientSocket.Client.LocalEndPoint.ToString());
                }
            }
            catch (Exception e)
            {
                if (OnSendStatusMSG != null)
                {
                    OnSendStatusMSG(e.Message);
                }

                if (OnDoDisconnect != null)
                {
                    OnDoDisconnect("连接异常...");
                }


                SocketDisconnect();


                return;
            }
        }

        private void ReadCallBack(IAsyncResult iar)
        {
            try
            {
                DataRead dataRead = (DataRead)iar.AsyncState;
                int recv = dataRead.networkStream.EndRead(iar);

                byte[] data = new byte[recv];
                Array.Copy(dataRead.msg, 0, data, 0, data.Length);

                if (isListen == true)
                {
                    if (OnSendRecMSG != null)
                    {
                        OnSendRecMSG(data);
                    }

                    dataRead = new DataRead(networkStream, clientSocket.ReceiveBufferSize);
                    networkStream.BeginRead(dataRead.msg, 0, dataRead.msg.Length, ReadCallBack, dataRead);
                }

            }
            catch (SocketException e)
            {
                if (OnSendStatusMSG != null)
                {
                    OnSendStatusMSG(e.Message);
                }
            }
            catch (IOException ioe)
            {
                if (OnSendStatusMSG != null)
                {
                    OnSendStatusMSG(ioe.Message);
                }

                SocketDisconnect();

            }
        }

        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public void SendData(string str)
        {
            try
            {
                byte[] bytesdata = HexToByte(str);
                networkStream.BeginWrite(bytesdata, 0, bytesdata.Length, new AsyncCallback(SendCallBack), networkStream);
                networkStream.Flush();
            }
            catch (Exception e)
            {
                if (OnSendStatusMSG != null)
                {
                    OnSendStatusMSG(e.Message);
                }
            }
        }

        public void SendData(byte[] data)
        {
            try
            {
                networkStream.BeginWrite(data, 0, data.Length, new AsyncCallback(SendCallBack), networkStream);
                networkStream.Flush();

                //Thread.Sleep(50);

            }
            catch (Exception e)
            {
                if (OnSendStatusMSG != null)
                {
                    OnSendStatusMSG(e.Message);
                }
            }
        }

        private void SendCallBack(IAsyncResult iar)
        {
            try
            {
                networkStream.EndWrite(iar);
            }
            catch (Exception e)
            {
                if (OnSendStatusMSG != null)
                {
                    OnSendStatusMSG(e.Message);
                }
            }
        }

    }

    internal class DataRead
    {
        public NetworkStream networkStream;
        public byte[] msg;
        public DataRead(NetworkStream ns, int buffersize)
        {
            this.networkStream = ns;
            msg = new byte[buffersize];
        }
    }
}
