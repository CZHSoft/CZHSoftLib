using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;								// Sleeping
using System.Net;									// Used to local machine info
using System.Net.Sockets;							// Socket namespace
using System.Collections;							// Access to the Array list

namespace CZHSoft.NetWork
{
    public class SocketServer
    {
        private ArrayList m_aryClients = new ArrayList();	// List of Client Connections
        private Socket listener = null;

        public delegate void GetStatusMSG(string strMsg);
        public event GetStatusMSG OnGetStatusMSG;

        public delegate void GetRecMSG(byte[] msg,string ip);
        public event GetRecMSG OnGetRecMSG;

        public delegate void SocketDisconnectDelegate(string msg,string ip);
        public event SocketDisconnectDelegate OnSocketDisconnect;

        public delegate void SocketConnectDelegate(string msg,string ip);
        public event SocketConnectDelegate OnSocketConnect;

        public delegate void SocketListenDelegate(string msg);
        public event SocketListenDelegate OnSocketListen;

        public delegate void SocketClientSendDelegate(string id,bool flag);
        public event SocketClientSendDelegate OnSocketClientSend;

        public delegate void SocketAllSendDelegate(string ip, bool flag);
        public event SocketAllSendDelegate OnSocketAllSend;

        public bool SocketListen(int port,int listenCount)
        {
            IPAddress[] aryLocalAddr = null;
            String strHostName = "";
            try
            {
                // NOTE: DNS lookups are nice and all but quite time consuming.
                strHostName = Dns.GetHostName();
                IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
                aryLocalAddr = ipEntry.AddressList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (OnGetStatusMSG != null)
                {
                    OnGetStatusMSG(ex.Message+"\n");
                }

                return false;
            }

            // Verify we got an IP address. Tell the user if we did
            if (aryLocalAddr == null || aryLocalAddr.Length < 1)
            {
                Console.WriteLine("Unable to get local address\r\n");

                if (OnGetStatusMSG != null)
                {
                    OnGetStatusMSG("Unable to get local address\r\n");
                }

                return false;
            }

            // Create the listener socket in this machines IP address
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(aryLocalAddr[0], port));  // For use with localhost 127.0.0.1 [0] [3]
            //Console.WriteLine(string.Format("buf size:{0}", listener.ReceiveBufferSize));
            listener.Listen(listenCount);

            // Setup a callback to be notified of connection requests
            listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);

            if (OnSocketListen != null)
            {
                OnSocketListen(string.Format("Listening on : [{0}] {1}:{2}\r\n", strHostName, aryLocalAddr[3], port));
            }

            return true;

        }

        public bool SocketListen(string ip,int port, int listenCount)
        {
            IPAddress ipAdd;
            string strHostName = "";
            try
            {
                // NOTE: DNS lookups are nice and all but quite time consuming.
                strHostName = Dns.GetHostName();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                if (OnGetStatusMSG != null)
                {
                    OnGetStatusMSG(ex.Message + "\n");
                }

                return false;
            }

            
            if (!IPAddress.TryParse(ip, out ipAdd))
            {
                throw new FormatException("Invalid ip-adress");
                return false;
            }

            // Create the listener socket in this machines IP address
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(ipAdd, port));  // For use with localhost 127.0.0.1 [0] [3]
            //Console.WriteLine(string.Format("buf size:{0}", listener.ReceiveBufferSize));
            listener.Listen(listenCount);

            // Setup a callback to be notified of connection requests
            listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);

            if (OnSocketListen != null)
            {
                OnSocketListen(string.Format("Listening on : [{0}] {1}:{2}\r\n", strHostName, ip, port));
            }

            return true;

        }


        private void OnConnectRequest(IAsyncResult ar)
        {
            try
            {
                Socket listener = (Socket)ar.AsyncState;
                NewConnection(listener.EndAccept(ar));
                listener.BeginAccept(new AsyncCallback(OnConnectRequest), listener);
            }
            catch
            {

            }
        }

        private void NewConnection(Socket sockClient)
        {
            // Program blocks on Accept() until a client connects.
            //SocketChatClient client = new SocketChatClient( listener.AcceptSocket() );
            SocketSessionClient client = new SocketSessionClient(sockClient);

            //client.Sock.SendTimeout = 1000;
            //client.Sock.ReceiveTimeout = 1000;

            m_aryClients.Add(client);
            
            if (OnSocketConnect != null)
            {
                OnSocketConnect(string.Format("Client {0}, joined...", client.Sock.RemoteEndPoint), client.Sock.RemoteEndPoint.ToString());
            }

            //Console.WriteLine(string.Format("Client {0}, joined\r\n", client.Sock.RemoteEndPoint));

            //// Get current date and time.
            //DateTime now = DateTime.Now;
            //String strDateLine = "Welcome " + now.ToString("G") + "\n\r";
            //// Convert to byte array and send.
            //Byte[] byteDateLine = System.Text.Encoding.ASCII.GetBytes(strDateLine.ToCharArray());
            //client.Sock.Send(byteDateLine, byteDateLine.Length, 0);

            client.SetupRecieveCallback(this);
        }

        public void OnRecievedData(IAsyncResult ar)
        {
            try
            {

                SocketSessionClient client = (SocketSessionClient)ar.AsyncState;
                byte[] aryRet = client.GetRecievedData(ar);

                // If no data was recieved then the connection is probably dead
                if (aryRet.Length < 1)
                {

                    if (OnSocketDisconnect != null)
                    {
                        OnSocketDisconnect(string.Format("Client {0}, disconnected\r\n", client.Sock.RemoteEndPoint), client.Sock.RemoteEndPoint.ToString());
                    }

                    //Console.WriteLine(string.Format("Client {0}, disconnected\r\n", client.Sock.RemoteEndPoint));

                    client.Sock.Close();
                    m_aryClients.Remove(client);
                    return;
                }

                //Console.WriteLine(aryRet.Length.ToString());


                if (OnGetRecMSG != null)
                {
                    OnGetRecMSG(aryRet, client.Sock.RemoteEndPoint.ToString());
                }
                // Send the recieved data to all clients (including sender for echo)
                //foreach (SocketChatClient clientSend in m_aryClients)
                //{
                //    try
                //    {
                //        clientSend.Sock.Send(aryRet);
                //    }
                //    catch
                //    {
                //        // If the send fails the close the connection
                //        Console.WriteLine(string.Format("Send to client {0} failed\r\n", client.Sock.RemoteEndPoint));

                //        if (OnSocketDisconnect != null)
                //        {
                //            OnSocketDisconnect(string.Format("Send to client {0} failed! Disconnected!\r\n", client.Sock.RemoteEndPoint));
                //        }

                //        clientSend.Sock.Close();
                //        m_aryClients.Remove(client);
                //        return;
                //    }
                //}
                client.SetupRecieveCallback(this);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public void SocketClose()
        {
            listener.Close();
        }

        public void SocketSend(byte[] sendData)
        {
            foreach (SocketSessionClient clientSend in m_aryClients)
            {
                int result=clientSend.Sock.Send(sendData);


                if (OnSocketAllSend != null)
                {
                    if (result == sendData.Length)
                    {
                        OnSocketAllSend(clientSend.Sock.RemoteEndPoint.ToString(), true);
                    }
                    else
                    {
                        OnSocketAllSend(clientSend.Sock.RemoteEndPoint.ToString(), false);
                    }
                }
            }
        }

        public void SocketSendWithIP(byte[] sendData,string ip,string id)
        {
            foreach (SocketSessionClient clientSend in m_aryClients)
            {
                if (clientSend.Sock.RemoteEndPoint.ToString() == ip)
                {
                    int result = clientSend.Sock.Send(sendData);

                    //Thread.Sleep(50);

                    if (OnSocketClientSend != null)
                    {
                        if (result == sendData.Length)
                        {
                            OnSocketClientSend(id, true);
                        }
                        else
                        {
                            OnSocketClientSend(id, false);
                        }
                    }
                }
            }
        }

        public void SocketSessionClose(string ip)
        {
            try
            {
                foreach (SocketSessionClient client in m_aryClients)
                {
                    if (client.Sock.RemoteEndPoint.ToString() == ip)
                    {
                        m_aryClients.Remove(client);

                        client.Sock.Close();

                        if (OnSocketDisconnect != null)
                        {
                            OnSocketDisconnect(string.Format("Client {0}, disconnected\r\n", ip), ip);
                        }

                        break;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


    }


    internal class SocketSessionClient
    {
        private Socket m_sock;						// Connection to the client
        private byte[] m_byBuff = new byte[300];		// Receive data buffer 1024
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sock">client socket conneciton this object represents</param>
        public SocketSessionClient(Socket sock)
        {
            m_sock = sock;
        }

        // Readonly access
        public Socket Sock
        {
            get { return m_sock; }
        }

        /// <summary>
        /// Setup the callback for recieved data and loss of conneciton
        /// </summary>
        /// <param name="app"></param>
        public void SetupRecieveCallback(SocketServer app)
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(app.OnRecievedData);
                m_sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Recieve callback setup failed! {0}\r\n", ex.Message));
            }
        }

        /// <summary>
        /// Data has been recieved so we shall put it in an array and
        /// return it.
        /// </summary>
        /// <param name="ar"></param>
        /// <returns>Array of bytes containing the received data</returns>
        public byte[] GetRecievedData(IAsyncResult ar)
        {
            int nBytesRec = 0;
            try
            {
                nBytesRec = m_sock.EndReceive(ar);
            }
            catch { }
            byte[] byReturn = new byte[nBytesRec];
            Array.Copy(m_byBuff, byReturn, nBytesRec);

            /*
            // Check for any remaining data and display it
            // This will improve performance for large packets 
            // but adds nothing to readability and is not essential
            int nToBeRead = m_sock.Available;
            if( nToBeRead > 0 )
            {
                byte [] byData = new byte[nToBeRead];
                m_sock.Receive( byData );
                // Append byData to byReturn here
            }
            */
            return byReturn;
        }
    }
}
