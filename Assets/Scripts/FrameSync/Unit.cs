using System;
using System.Collections;
using Client.Library.Entity;
using DG.Tweening;
using UnityEngine;

namespace Assets.Scripts.FrameSync {
    public class UnitData {
        public int Hp = 100;
        public int Attack = 20;
    }

    public class Unit : EntityBase {

        private const int MOVE_STEP = 1;//每次前后移动1米
        private const int MOVE_ANGLE = 15;//每次左右旋转15度
        private const int DURATION = 20;//20毫秒内完成

        private string _guid;//系统分配的玩家ID
        private bool IsMe { set;  get; }

        protected override int ThisType {
            get { return EntityConsts.EntityType.Unit; }
        }

        protected override void RegisterReceiveActions() {
            SubcribeAction(EntityConsts.Message.UNIT_MOVE, OnUnitMove);
        }

        private void OnUnitMove(Hashtable obj) {
            string guid = (string) obj["guid"];
            if (guid != _guid) return;
            string actionInfo = (string) obj["action"];
            EMoveActionType actionType = (EMoveActionType) int.Parse(actionInfo);
            //Debug.LogFormat("actionInfo, {0}", actionType);
            DoNewAction(actionType);
        }

        private void DoNewAction(EMoveActionType actionType) {
            switch (actionType) {
                case EMoveActionType.Forward:
                    DoMove(1);
                    break;
                case EMoveActionType.Back:
                    DoMove(-1);
                    break;
                case EMoveActionType.Left:
                    DoRotate(-1);
                    break;
                case EMoveActionType.Right:
                    DoRotate(1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("actionType", actionType, null);
            }
        }

        private void DoRotate(int p) {
            Vector3 targetAngle = transform.eulerAngles + Vector3.up * p * MOVE_ANGLE;
            transform.DORotate(targetAngle, DURATION / 1000f);
        }

        private void DoMove(int p) {
            Vector3 targetPos = transform.position + transform.forward * MOVE_STEP * p;
            transform.DOMove(targetPos, DURATION / 1000f);
        }


        public void Init(bool isMe, int unitId, string guid) {
            IsMe = isMe;
            _guid = guid;
            transform.position = Vector3.right * unitId * 4; 
        }
    }
}