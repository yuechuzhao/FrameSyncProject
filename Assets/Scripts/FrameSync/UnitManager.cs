using System.Collections;
using Client.Library.Entity;
using UnityEngine;

namespace Assets.Scripts.FrameSync {
    public class UnitManager : EntityBase {
        protected override int ThisType {
            get { return EntityConsts.EntityType.UnitManager; }
        }

        protected override void OnCreate(object param = null) {
            Send(EntityConsts.Message.JOIN_GAME, null);
        }

        private void CreateUnit(bool isMe, string guid, int unitId) {
            var go = GameObject.Instantiate(Resources.Load<GameObject>("Unit"));
            var unit = EntityBase.Create<Unit>(go, EntityPool.Instance);
            unit.Init(isMe, unitId, guid);
        }

        protected override void RegisterReceiveActions() {
            SubcribeAction(EntityConsts.Message.CREATE_UNIT, OnCreateUnit);
        }

        private void OnCreateUnit(Hashtable obj) {
            Debug.LogFormat("创建新角色");
            bool isMe = (bool) obj["isMe"];
            int unitId = (int) obj["unitId"];
            string guid = (string) obj["guid"];
            CreateUnit(isMe, guid, unitId);
        }
    }
}