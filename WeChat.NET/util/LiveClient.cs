using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;

namespace WeChat.NET.util
{
    class LiveClient
    {
        private static Socket socketClient = null;
        private static int socketTimeout = 20; //20秒
        private static String ipTxt = "127.0.0.1";
        private static String portTxt = "1234";


        //  start 
        static int bufsize = 1024;
        const int bufsizelimit = 1024 * 1024; //TO DO: buffer too small
        static byte[] buffer = new byte[bufsize < bufsizelimit ? bufsizelimit : bufsize];
        static int bufferused = 0;

        public static void PrintData(string input)
        {
            byte[] bt = System.Text.Encoding.Unicode.GetBytes(input);
            PrintData(bt);
        }

        public static void PrintData(byte[] pBuffer)
        {
            FileStream fs = null;
            BinaryWriter bw = null;
            try
            {
                fs = new FileStream("wechat.log", FileMode.Append);
                bw = new BinaryWriter(fs);
                bw.Write(pBuffer, 0, pBuffer.Length);
                bw.Flush();
            }
            catch(Exception e)
            {
                //TO DO: catch
                
            }
            finally
            {
                bw.Close();
                fs.Close();
            }
        }

        public static void SendData(string input)
        {
            byte[] bt = System.Text.Encoding.Unicode.GetBytes(input);
            SendData(bt);
        }

        public static void SendData(byte[] pBuffer)
        {
            try
            {
                int ret = SocketSendData(pBuffer, pBuffer.Length, socketTimeout);
            }
            catch 
            {
                //TO DO: exception handling
            }
        }
        //  end 



        public static int ConnectServer()
        {
            return ConnectServer(ipTxt, portTxt);
        }

        public static int ConnectServer(String ipTxt, String portTxt)
        {
            LiveClient.ipTxt = ipTxt;
            LiveClient.portTxt = portTxt;
            IPAddress address = IPAddress.Parse(ipTxt.Trim());
            IPEndPoint endpoint = new IPEndPoint(address, int.Parse(portTxt.Trim()));
            try
            {
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketClient.Connect(endpoint);

            }
            catch 
            {
                //log.Debug("[" + socketClient.RemoteEndPoint + "] Connection Failed. Reason:" + e.Message);
                return -1;
            }

            if (socketClient.Connected)
            {
                //log.Debug("[" + socketClient.RemoteEndPoint + "] Connection Successful");
                return 1;
            }
            else
            {
                //log.Debug("[" + socketClient.RemoteEndPoint + "] Connection Failed  Reason Unknown.");
                return -1;
            }
            
        }

        public static int SocketSendData(byte[] buffer, int length, int outTime)
        {
            if (socketClient == null || socketClient.Connected == false)
            {
                throw new ArgumentException("未连接到远程计算机");
            }
            if (buffer == null || buffer.Length == 0 || length == 0)
            {
                throw new ArgumentException("参数buffer 为null ,或者长度为 0");
            }

            int flag = 0;
            try
            {
                int left = length;
                int sndLen = 0;

                while (true)
                {
                    if ((socketClient.Poll(outTime * 1000000, SelectMode.SelectWrite) == true))
                    {
                        sndLen = socketClient.Send(buffer, sndLen, left, SocketFlags.None);
                        left -= sndLen;
                        if (left == 0)
                        {                                        // 数据已经全部发送
                            flag = 0;
                            break;
                        }
                        else
                        {
                            if (sndLen > 0)
                            {                                    // 数据部分已经被发送
                                continue;
                            }
                            else
                            {                                                // 发送数据发生错误
                                flag = -2;
                                break;
                            }
                        }
                    }
                    else
                    {                                                        // 超时退出
                        flag = -1;
                        break;
                    }
                }
            }
            catch
            {
                ConnectServer();
                flag = -3;
            }
            return flag;
        }
    }
}