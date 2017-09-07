using System.Collections;
using Client.Library.Entity;
using UnityEngine;

namespace Assets.Scripts.FrameSync {
    public static class FrameSyncOperation {
        public const int UNIT_CREATE = 1;
    }

    public class ClientFrameController : EntityBase {

        public const float SYNC_MS = 100;
        private float _elapsedTime = 0;
        private int _currentFrame = 0;
        private string _uuid;//用来标识本机

        protected override int ThisType {
            get { return EntityConsts.EntityType.FrameController; }
        }

        protected override void RegisterReceiveActions() {
            SubcribeAction(EntityConsts.Message.UNIT_CREATED, SendUnityCreated);
        }

        protected override void OnCreate(object param = null) {
            _uuid = System.Guid.NewGuid().ToString();
        }

        private void SendUnityCreated(Hashtable obj) {
            
        }

        private void Update() {
            _elapsedTime += Time.unscaledDeltaTime;
            if (_elapsedTime >= SYNC_MS) {
                FixTick();
                _elapsedTime = 0f;
            }
        }

        private void FixTick() {
            _currentFrame++;//进行一帧
            Debug.LogFormat("当前帧,{0}", _currentFrame);
            ExecuteOperations();
        }

        /// <summary>
        /// 执行之后的操作
        /// </summary>
        private void ExecuteOperations() {
        }

        /// <summary>
        /// 发送本机操作
        /// </summary>
        private void SendOperations() {
        }
    }
}