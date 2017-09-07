using UnityEngine;

namespace Assets.Scripts.UDPClient {
    public class UDPClientTester : MonoBehaviour {

        private void OnGUI() {
            if (GUI.Button(new Rect(0, 0, 120, 30), "测试发送")) {
                MyUDPClient.Send("test udp connection");
            }
        }

        private void Start() {
            GameObject go = new GameObject(typeof(MyUDPClient).Name);
            go.AddComponent<MyUDPClient>();
            MyUDPClient.OnNewDataReceived = OnNewMsgReceived;
        }

        private void OnNewMsgReceived(string obj) {
            Debug.LogFormat("OnNewMsgReceived: {0}", obj);
        }
    }
}