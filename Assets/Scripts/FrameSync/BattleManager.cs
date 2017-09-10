using System.Collections;
using Assets.Scripts.UDPClient;
using Client.Library.Entity;
using UnityEngine;

namespace Assets.Scripts.FrameSync {
    public class BattleManager : MonoBehaviour {

        private void Start() {
            Init();
        }

        public void StartGame() {
            StartCoroutine(CreateUnitManager());
        }

        private IEnumerator CreateUnitManager() {
            yield return null;//等待udpclient?
            
            // 先同步当前服务器的帧
            var go = new GameObject(typeof(ClientFrameController).Name);
            EntityBase.Create<ClientFrameController>(go, EntityPool.Instance);

            go = new GameObject(typeof(UnitManager).Name);
            EntityBase.Create<UnitManager>(go, EntityPool.Instance);
        }

        private void Init() {
            var panelRoot = Instantiate(Resources.Load<GameObject>("BattlePanel"), Launcher.UIRoot, false);
            EntityBase.Create<UIController>(panelRoot.gameObject, EntityPool.Instance);
            

            new FrameProcessor(EntityPool.Instance);


        }
    }
}