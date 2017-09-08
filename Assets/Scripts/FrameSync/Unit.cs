using System.Collections;
using Client.Library.Entity;
using UnityEngine;

namespace Assets.Scripts.FrameSync {

    public class Unit : EntityBase {
        private string _guid;//系统分配的玩家ID
        private bool IsMe { set;  get; }

        protected override int ThisType {
            get { return EntityConsts.EntityType.Unit; }
        }

        protected override void RegisterReceiveActions() {
        }


        public void Init(bool isMe, int unitId, string guid) {
            IsMe = isMe;
            _guid = guid;
            transform.position = Vector3.right * unitId; 
        }
    }
}