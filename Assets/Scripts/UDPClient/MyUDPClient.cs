using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.UDPClient {
    public class MyUDPClient : MonoBehaviour
    {

        private static IPAddress GroupAddress = IPAddress.Parse("192.168.199.223");

        private static int GroupPort = 21003;
        public static System.Action<string> OnNewDataReceived;

        private static UdpClient _client;
        private static string _receivedMsg;

        /// <summary>
        /// 初始化时先创建新的
        /// </summary>
        public MyUDPClient() {
            _client = new UdpClient();
            Thread t = new Thread(ReciveMsg);
            t.Start();
        }

        public static void Send(string message)
        {
            IPEndPoint groupEP = new IPEndPoint(GroupAddress, GroupPort);

            try
            {
                UnityEngine.Debug.LogFormat("Sending datagram : {0}", message);

                byte[] bytes = Encoding.ASCII.GetBytes(message);

                _client.Send(bytes, bytes.Length, groupEP);

                _client.Close();

            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }

        }


        /// <summary>
        /// 接收发送给本机ip对应端口号的数据报
        /// </summary>
        static void ReciveMsg()
        {
            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var listenClient = new UdpClient(listenEndPoint);
            while (true)
            {
                UnityEngine.Debug.LogFormat("receive data................");
                IPEndPoint point = new IPEndPoint(GroupAddress, GroupPort);//用来保存发送方的ip和端口号
                byte[] buffer = listenClient.Receive(ref point);//接收数据报
                _receivedMsg = System.Text.Encoding.Default.GetString( buffer );
            }
        }

        private static void ReceiveData(IAsyncResult ar) {
            throw new NotImplementedException();
        }

        private void Update() {
            if (string.IsNullOrEmpty(_receivedMsg)) return;
            if (OnNewDataReceived != null) {
                OnNewDataReceived(_receivedMsg);
                _receivedMsg = null;
            }

        }
    }
}