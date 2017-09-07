using Client.Library.Entity;
using UnityEngine;

namespace Assets.Scripts.FrameSync {
    public class FrameSyncDemo : MonoBehaviour {

        private void Start() {
            Init();
        }

        private void OnGUI() {
            if (GUI.Button(new Rect(0, 0, 120, 30), "开始游戏")) {
                CreateSelf();
            }
        }

        private void CreateSelf() {
            var go = GameObject.Instantiate(Resources.Load<GameObject>("Unit"));
            go.SetActive(false);
            var unit = EntityBase.Create<Unit>(go, EntityPool.Instance);
            unit.IsPlayer = true;
        }

        private void Init() {
            var go = new GameObject(typeof(ClientFrameController).Name);
            EntityBase.Create<ClientFrameController>(go, EntityPool.Instance);

            var panelRoot = GameObject.Find("UICanvas/ButtonPanel");
            EntityBase.Create<UIController>(panelRoot.gameObject, EntityPool.Instance);
        }
    }
}