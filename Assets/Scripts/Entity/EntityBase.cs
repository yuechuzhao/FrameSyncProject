using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Library.Entity {
    public class EntityBase : MonoBehaviour, IEntity {
        public const int WildEntityType = -999;//通配类型
        public int ObjectId {get { return Trans.GetInstanceID();}}

        public IEntityPool PoolInstance { set; private get; }

        
        protected virtual int ThisType {
            get { return WildEntityType; }
        }

        private Transform _trans;
        protected Transform Trans {
            get {
                return _trans ?? (_trans = transform);
            }
        }

        private Dictionary<string, System.Action<Hashtable>> _receiveActions = new Dictionary<string, Action<Hashtable>>();

        public static T Create<T> (GameObject go, IEntityPool pool, object param = null) where T : EntityBase {
            var instance = go.AddComponent<T>();
            instance.PoolInstance = pool;
            instance.Register();
            instance.OnCreate(param);
            instance.RegisterReceiveActions();
            return instance;
        }


        /// <summary>
        /// entity里面常用的检查本实体是否目标实体
        /// </summary>
        /// <param name="args"></param>
        /// <param name="entity"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool CheckTargetIdIsSelf (Hashtable args, IEntity entity, string key = "targetId") {
            int targetId = 0;
            if (args.ContainsKey(key)) {
                targetId = (int)args[key];
            }
            return targetId == entity.ObjectId;
        }

        #region IEntity implementation

        public bool IsEntity (int id) {
            return ObjectId == id;
        }

        public int EntityType() {
            return ThisType;
        }

        /// <summary>
        /// 向实体池注册此实体
        /// </summary>
        public void Register () {
            PoolInstance.Register(this);
        }

        /// <summary>
        /// 向实体池反注册此实体
        /// </summary>
        public void UnRegister () {
            PoolInstance.UnRegister(this);
            OnRemove();
        }

        /// <summary>
        /// 发送事件信息
        /// </summary>
        /// <param name="actionName">事件名</param>
        /// <param name="args">事件参数</param>
        public void Send (string actionName, Hashtable args) {
            PoolInstance.Deal(this, actionName, args);
        }

        /// <summary>
        /// 接收事件信息
        /// </summary>
        /// <param name="actionName">事件名</param>
        /// <param name="args">事件参数</param>
        public void Receive(string actionName, Hashtable args) {
            if (!_receiveActions.ContainsKey(actionName)) return;
            _receiveActions[actionName](args);
        }

        /// <summary>
        /// 替代以前的receive，直接添加方法
        /// </summary>
        protected virtual void RegisterReceiveActions() {
        }

        protected virtual void OnCreate (object param = null) { }

        protected virtual void OnRemove () { }
        #endregion

        protected virtual void OnDestroyed () { }

        protected void SubcribeAction(string actionName, System.Action<Hashtable> action) {
            if (!_receiveActions.ContainsKey(actionName)) {
                _receiveActions.Add(actionName, action);
            }
        }

        private void OnDestroy () {
            if (PoolInstance != null) {
                UnRegister();
            }
            OnDestroyed();
        }
    }
}
