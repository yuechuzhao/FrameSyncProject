using Assets.Scripts.UDPClient;
using Client.Library.Entity;
using UnityEngine;

namespace Assets.Scripts.FrameSync {
    public class FrameSyncDemo : MonoBehaviour {

        private void Start() {
            Init();
        }

        private void OnGUI() {
            if (GUI.Button(new Rect(0, 0, 120, 30), "开始游戏")) {
                StartGame();
            }
        }

        private void StartGame() {

            var go = new GameObject(typeof(UnitManager).Name);
            EntityBase.Create<UnitManager>(go, EntityPool.Instance);
        }

        private void Init() {
            var go = new GameObject(typeof(ClientFrameController).Name);
            EntityBase.Create<ClientFrameController>(go, EntityPool.Instance);

            var panelRoot = GameObject.Find("UICanvas/ButtonPanel");
            EntityBase.Create<UIController>(panelRoot.gameObject, EntityPool.Instance);
            
            go = new GameObject(typeof(MyUDPClient).Name);
            go.AddComponent<MyUDPClient>();
            new FrameProcessor(EntityPool.Instance);
        }
    }
}