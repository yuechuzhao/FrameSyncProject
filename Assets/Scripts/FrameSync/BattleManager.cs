using Assets.Scripts.UDPClient;
using Client.Library.Entity;
using UnityEngine;

namespace Assets.Scripts.FrameSync {
    public class BattleManager : MonoBehaviour {

        private void Start() {
            Init();
        }

        public void StartGame() {
            var go = new GameObject(typeof(UnitManager).Name);
            EntityBase.Create<UnitManager>(go, EntityPool.Instance);
        }

        private void Init() {


            var go = new GameObject(typeof(ClientFrameController).Name);
            EntityBase.Create<ClientFrameController>(go, EntityPool.Instance);

            var panelRoot = Instantiate(Resources.Load<GameObject>("InputPanel"), Launcher.UIRoot, false);
            EntityBase.Create<UIController>(panelRoot.gameObject, EntityPool.Instance);
            

            new FrameProcessor(EntityPool.Instance);


        }
    }
}