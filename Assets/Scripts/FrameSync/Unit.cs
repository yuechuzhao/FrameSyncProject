using System.Collections;
using Client.Library.Entity;
using UnityEngine;

namespace Assets.Scripts.FrameSync {

    public class Unit : EntityBase {
        private string _uuid;//系统分配的玩家ID
        private bool IsPlayer { set;  get; }

        protected override int ThisType {
            get { return EntityConsts.EntityType.Player; }
        }

        protected override void OnCreate(object param = null) {
            IsPlayer = (bool) param;
            if (IsPlayer) {
                Send(EntityConsts.Message.UNIT_CREATED, null);
            }
        }

        protected override void RegisterReceiveActions() {
            SubcribeAction(EntityConsts.Message.ACTIVATE_SELF, OnActivateSelf);
        }

        private void OnActivateSelf(Hashtable obj) {
            Debug.LogFormat("激活自己");
            int unitId = (int) obj["unitId"];
            gameObject.SetActive(true);
            transform.position = Vector3.right * unitId; 
        }
    }
}