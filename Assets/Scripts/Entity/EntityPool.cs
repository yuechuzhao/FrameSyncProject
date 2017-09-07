using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;


namespace Client.Library.Entity {
    public class EntityPool : IEntityPool {

        public delegate void TriggerFunction (int targetType, string actionName, Hashtable args);

        public delegate bool TriggerCallback (
            IEntity sender,
            string actionName,
            TriggerFunction function,
            Hashtable args
            );

        private static EntityPool _instance;

        public static EntityPool Instance {
            get { return _instance ?? (_instance = new EntityPool()); }
        }

        private readonly List<IEntity> _entities = new List<IEntity>();
        // 大量使用的delegate，不使用+=而用list来记录，可以减少gc
        private readonly List<TriggerCallback> _callbacks = new List<TriggerCallback>();

        public void AddTrigger (TriggerCallback trigger) {
            if (!_callbacks.Contains(trigger)) {
                _callbacks.Add(trigger);
            }
        }

        public void RemoveTrigger (TriggerCallback trigger) {
            _callbacks.Remove(trigger);
        }

        public void RemoveAllTrigger () {
            _callbacks.Clear();
        }
        #region IEntityPool implementation

        public void Register (IEntity entity) {
            _entities.Add(entity);
        }

        public void UnRegister (IEntity entity) {
            _entities.Remove(entity);
        }

        public void UnRegisterAll () {
            for (int i = 0; i < _entities.Count; i++) {
                var entity = (_entities[i] as EntityBase);
                Object.Destroy(entity);
                if (entity != null) {
                    Object.Destroy(entity.gameObject);
                }
            }
            _entities.Clear();
        }

        public void Reset () {
            UnRegisterAll();
            RemoveAllTrigger();
            _instance = null;
        }

        public void Deal (IEntity entity, string actionName, Hashtable args) {
            //Debug.LogFormat("Deal {0} action {1} {2} {3}", entity.GetType().Name, actionName, args[0], args[1]);
            for (int index = 0; index < _callbacks.Count; index++) {
                _callbacks[index](entity, actionName, CastToTargetsByType, args);
            }
        }

        private void CastToTargetsByType (int targetType, string actionName, Hashtable args) {
            for (int i = 0; i < _entities.Count; i++) {
                if (targetType == EntityBase.WildEntityType || _entities[i].EntityType() == targetType) {
                    //Debuger.LogFormat("CastToTargetsByType {0} ", targetType, LogLevel.None);
                    _entities[i].Receive(actionName, args);
                }
            }
        }

        private List<IEntity> GetTargetsByType (int targetType) {
            var ret = new List<IEntity>();
            for (int i = 0; i < _entities.Count; i++) {
                //			Debug.LogFormat ("CastToTargetsByType target {0} current type {1}", targetType, _entities[i].EntityType());
                if (_entities[i].EntityType() == (targetType)) {
                    ret.Add(_entities[i]);
                }
            }
            return ret;
        }

        public IEntity GetTargetById (int id, int entityType) {
            for (int i = 0; i < _entities.Count; i++) {
                var entity = _entities[i];
                if (CheckIsTargetEntity(id, entityType, entity)) {
                    return entity;
                }
            }
            return null;
        }

        private static bool CheckIsTargetEntity (int id, int entityType, IEntity entity) {
            return entity != null && entity.IsEntity(id) && entity.EntityType() == (entityType);
        }

        #endregion
    }
}