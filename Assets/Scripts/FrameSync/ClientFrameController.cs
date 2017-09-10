using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UDPClient;
using Client.Library.Entity;
using UnityEngine;

namespace Assets.Scripts.FrameSync {
    public class ClientFrameController : EntityBase {

        public const float SYNC_MS = 66;//固定帧，每66毫秒前进一帧
        private float _elapsedTime = 0;
        private int _currentFrame = 0;
        private string _guid;//用来标识本机

        private List<ClientMsg> _msgs;
        
        private readonly Dictionary<int, System.Action<ClientMsg>> _serverCallbacks = 
            new Dictionary<int, Action<ClientMsg>>() { };

        protected override int ThisType {
            get { return EntityConsts.EntityType.FrameController; }
        }

        protected override void RegisterReceiveActions() {
            SubcribeAction(EntityConsts.Message.JOIN_GAME, SendUnityCreated);
            SubcribeAction(EntityConsts.Message.PLAY_OPERATION, SendPlayerOperation);
        }

        protected override void OnCreate(object param = null) {
            MyUDPClient.OnNewDataReceived += OnNewDataReceived;
            _guid = System.Guid.NewGuid().ToString();
            Debug.LogFormat("guid is {0}", _guid);
            
            // 
            InitDataStructures();
            RequestServerFrame();
        }



        protected override void OnRemove() {
            MyUDPClient.OnNewDataReceived -= OnNewDataReceived;
        }
        
        private void RequestServerFrame() {
            var msg = new RequestServerFrameMsg() {};
            SendClientMsg(msg);
        }

        private void SendPlayerOperation(Hashtable obj) {
            string operation = (string) obj["operation"];
            operation = operation.ParseToProtoString();
            //Debug.LogFormat("SendPlayerOperation, {0}", operation);
            var msg = new UnitMoveMsg {OperationInfo = operation};
            SendClientMsg(msg);
        }

        private void ReceiveSyncData(ClientMsg msg) {
            if (_msgs == null) {
                _msgs = new List<ClientMsg>();
                ExecuteMsg(msg);//第一次是同步服务器的逻辑帧，要优先处理
            }
            bool alreadyReceived = false;
            for (int index = 0; index < _msgs.Count; index++) {
                if (_msgs[index].SendId != msg.SendId) continue;
                alreadyReceived = true;
                break;
            }
            if (!alreadyReceived) {
                Debug.LogFormat("new msg add ,{0}, operationCode {1}", msg.SendId, msg.OperationCode);
                _msgs.Add(msg);
            }
        }
        
        
        private void InitDataStructures() {
            _serverCallbacks.Add(ClientMsg.SERVER_FRAME, OnServerFrameGot);
            _serverCallbacks.Add(ClientMsg.CREATION, OnUnitCreation);
            _serverCallbacks.Add(ClientMsg.MOVE, OnUnitMove);
        }

        private void OnNewDataReceived(string obj) {
            string[] msgs = obj.Split(ClientMsg.FIRST_SPLITER);
            for (int seqIndex = 0; seqIndex < msgs.Length; seqIndex++) {
                var syncData = ClientMsg.ParseFrom(msgs[seqIndex]);
                ReceiveSyncData(syncData);
            }
        }

        private void OnServerFrameGot(ClientMsg msg) {
            _currentFrame = msg.FrameId;
            Debug.LogFormat("OnServerFrameGot, frameId {0}", msg.FrameId);
            StartCoroutine(StartFrameSync());
        }

        private void OnUnitCreation(ClientMsg msg) {
            bool isMe = msg.Guid == _guid;
            Send(EntityConsts.Message.CREATE_UNIT, new Hashtable() {
                {"unitId", int.Parse(msg.OperationInfoStr)},
                {"guid", msg.Guid},
                {"isMe", isMe}
            });
        }

        private void OnUnitMove(ClientMsg msg) {
            Send(EntityConsts.Message.UNIT_MOVE, new Hashtable() {
                {"action", msg.OperationInfoStr},
                {"guid", msg.Guid},
            });
        }
        

        private void ExecuteMsg(ClientMsg msg) {
            //Debug.LogFormat("msg is {0}, guid {1}", msg.OperationCode, msg.Guid);
            System.Action<ClientMsg> callback;
            if (_serverCallbacks.TryGetValue(msg.OperationCode, out callback)) {
                callback(msg);
            }
        }

        private void SendUnityCreated(Hashtable obj) {
            Debug.LogFormat("Send Unity Create");
            SendClientMsg(new UnitCreationMsg());
        }

        private IEnumerator StartFrameSync() {
            _elapsedTime = 0;
            while (true) {
                yield return null;
                _elapsedTime += Time.unscaledDeltaTime;
                if (_elapsedTime >= SYNC_MS / 1000f) {
                    FixTick();
                    _elapsedTime = 0f;
                }
            }
        }

        private void FixTick() {
            _currentFrame++;//进行一帧
 //           Debug.LogFormat("当前帧,{0}", _currentFrame);
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
        private void SendClientMsg(ClientMsg data) {
            Debug.LogFormat("SendClientMsg, operation {0}, frame at {1}", data.OperationCode, data.FrameId);
            data.Guid = _guid;
            MyUDPClient.SendMessage(data);
        }
    }
}