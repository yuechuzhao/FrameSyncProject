using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UDPClient;
using Client.Library.Entity;
using UnityEngine;

namespace Assets.Scripts.FrameSync {
    public class ClientFrameController : EntityBase {

        public const float SYNC_MS = 100;
        private float _elapsedTime = 0;
        private int _currentFrame = 0;
        private string _guid;//用来标识本机
        
        private List<ClientMsg> _msgs = new List<ClientMsg>();

        protected override int ThisType {
            get { return EntityConsts.EntityType.FrameController; }
        }

        protected override void RegisterReceiveActions() {
            SubcribeAction(EntityConsts.Message.JOIN_GAME, SendUnityCreated);
            SubcribeAction(EntityConsts.Message.PLAY_OPERATION, SendPlayerOperation);
        }

        protected override void OnCreate(object param = null) {
            _guid = System.Guid.NewGuid().ToString();
            MyUDPClient.OnNewDataReceived += OnNewDataReceived;
        }

        protected override void OnRemove() {
            MyUDPClient.OnNewDataReceived -= OnNewDataReceived;
        }

        private void SendPlayerOperation(Hashtable obj) {
            string operation = (string) obj["operation"];
            operation = operation.ParseToProtoString();
            var msg = new UnitMoveMsg {OperationInfo = operation};
            SendOperation(msg);
        }

        private void ReceiveSyncData(ClientMsg msg) {
            bool alreadyReceived = false;
            for (int index = 0; index < _msgs.Count; index++) {
                if (_msgs[index].SendId != msg.SendId) continue;
                alreadyReceived = true;
                break;
            }
            if (!alreadyReceived) {
                Debug.LogFormat("new msg add ,{0}", msg.SendId);
                _msgs.Add(msg);
            }
        }

        private void OnNewDataReceived(string obj) {
            var syncData = ClientMsg.ParseFrom(obj);
            ReceiveSyncData(syncData);
        }

        private void ExecuteMsg(ClientMsg msg) {
            Debug.LogFormat("msg is {0}, guid {1}", msg.OperationCode, msg.Guid);
            switch (msg.OperationCode){
                case ClientMsg.CREATION:
                    bool isMe = msg.Guid == _guid;
                    Send(EntityConsts.Message.CREATE_UNIT, new Hashtable() {
                        {"unitId", int.Parse(msg.OperationInfoStr)},
                        {"guid", msg.Guid},
                        {"isMe", isMe}
                    });
                    break;
                case ClientMsg.MOVE:
                    Send(EntityConsts.Message.UNIT_MOVE, new Hashtable() {
                        {"action", Enum.Parse(typeof(EMoveActionType), msg.OperationInfoStr)},
                        {"guid", msg.Guid},
                    });
                    break;
                default:
                    break;
            }
        }

        private void SendUnityCreated(Hashtable obj) {
            Debug.LogFormat("Send Unity Create");
            SendOperation(new UnitCreationMsg());
        }

        private void Update() {
            _elapsedTime += Time.unscaledDeltaTime;
            if (_elapsedTime >= SYNC_MS / 1000f) {
                FixTick();
                _elapsedTime = 0f;
            }
        }

        private void FixTick() {
            _currentFrame++;//进行一帧
//            Debug.LogFormat("当前帧,{0}", _currentFrame);
            ExecuteOperations();
        }

        /// <summary>
        /// 执行之后的操作
        /// </summary>
        private void ExecuteOperations() {
            for (int index = _msgs.Count - 1; index >= 0; index--) {
                var msg = _msgs[index];
                if (msg.FrameId <= _currentFrame) {
                    ExecuteMsg(msg);
                    _msgs.Remove(msg);
                }
            }
        }

        /// <summary>
        /// 发送本机操作
        /// </summary>
        private void SendOperation(ClientMsg data)
        {
            data.Guid = _guid;
            data.FrameId = _currentFrame;
            MyUDPClient.SendMessage(data);
        }
    }
}