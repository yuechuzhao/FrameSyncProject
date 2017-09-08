using UnityEngine;

namespace Assets.Scripts {
    public class Launcher : MonoBehaviour {

        private static Transform _uiRoot;
        public static Transform UIRoot {
            get { return _uiRoot ?? (_uiRoot = GameObject.Find("UICanvas").transform); }
        }

        void Start() {
            var choosePanel = Instantiate(Resources.Load<GameObject>("ServerChoosePanel"), UIRoot, false);
            choosePanel.AddComponent<ServerChoosePanel>();
        }
    }
}