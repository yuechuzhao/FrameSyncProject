using Client.Library.Entity;
using UnityEngine;

namespace Assets.Scripts.FrameSync {

    public class Unit : EntityBase {
        private int _UnitId = 0;//系统分配的玩家ID
        public bool IsPlayer { set; private get; }

        protected override int ThisType {
            get { return EntityConsts.EntityType.Player; }
        }

        protected override void OnCreate(object param = null) {
            if (IsPlayer) {
                Send(EntityConsts.Message.UNIT_CREATED, null);
            }
        }
    }
}