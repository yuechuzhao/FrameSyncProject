using Assets.Scripts.FrameSync;
using Assets.Scripts.UDPClient;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
    public class ServerChoosePanel : MonoBehaviour {

        private string _inputString;
        private string _ip;
        private int _port;

        private void Awake() {
            var input = transform.Find("ServerSet").GetComponentInChildren<InputField>();
            input.text = string.Format("{0}:{1}", UDPSetting.ServerIP, UDPSetting.ServerPort);
            input.onValueChanged.AddListener(OnInput);
            _inputString = input.text;

            var button = transform.GetComponentInChildren<Button>();
            button.onClick.AddListener(OnClickButton);
        }

        private void OnClickButton() {
            if (!TrySaveServerIp()) return;
            gameObject.SetActive(false);
            var go = new GameObject(typeof(MyUDPClient).Name);
            go.AddComponent<MyUDPClient>();

            go = new GameObject(typeof(BattleManager).Name, typeof(BattleManager));
            var battle = go.GetComponent<BattleManager>();
            battle.StartGame();
        }

        private bool TrySaveServerIp() {
            string[] array = _inputString.Split(':');
            if (array.Length != 2) {
                Debug.LogError("输入的ip地址错误,应为xx.xx.xx.xx:xxxx格式!");
                return false;
            }
            UDPSetting.ServerIP = array[0];
            UDPSetting.ServerPort = int.Parse(array[1]);
            return true;
        }

        private void OnInput(string text) {
            _inputString = text;
        }
    }
}