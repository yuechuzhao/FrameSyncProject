using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Assets.Scripts.FrameSync;
using UnityEngine;

namespace Assets.Scripts.UDPClient {
    public class MyUDPClient : MonoBehaviour {
        private static IPAddress GroupAddress;

        private static int GroupPort;
        private static int ListenPort;
        public static System.Action<string> OnNewDataReceived;

        private static UdpClient _client;
        private static string _receivedMsg;

        private static Thread _recThread;

        /// <summary>
        /// 初始化时先创建新的
        /// </summary>
        public MyUDPClient() {
            _client = new UdpClient(ListenPort);

        }

        public static void Send(string message)
        {
            
            IPEndPoint groupEP = new IPEndPoint(GroupAddress, GroupPort);

            try
            {
                //UnityEngine.Debug.LogFormat("Sending datagram : {0}", message);

                byte[] bytes = Encoding.ASCII.GetBytes(message);

                _client.Send(bytes, bytes.Length, groupEP);
                _recThread= new Thread(ReciveMsg);
                _recThread.Start();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }

        }

        public static void SendMessage(ClientMsg data) {
            Send(data.ToString());
        }

        /// <summary>
        /// 接收发送给本机ip对应端口号的数据报
        /// </summary>
        static void ReciveMsg()
        {
            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, ListenPort);
            while (true)
            {
                byte[] buffer = _client.Receive(ref listenEndPoint);//接收数据报

                //UnityEngine.Debug.LogFormat("received from {0}, {1}", listenEndPoint.Address, listenEndPoint.Port);
                _receivedMsg = System.Text.Encoding.Default.GetString( buffer );
            }
        }

        private void Update() {
            if (string.IsNullOrEmpty(_receivedMsg)) return;
            if (OnNewDataReceived != null) {
                OnNewDataReceived(_receivedMsg);
                _receivedMsg = null;
            }
        }

        private void Awake() {
            GroupAddress = IPAddress.Parse(UDPSetting.ServerIP);
            GroupPort = UDPSetting.ServerPort;
            ListenPort = UDPSetting.LocalPort;
        }

        void OnDestroy() {
            _client.Close();
        }
    }
}