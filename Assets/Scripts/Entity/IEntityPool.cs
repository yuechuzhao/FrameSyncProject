using System.Collections;

namespace Client.Library.Entity {
    public interface IEntityPool {
        void Register (IEntity entity);
        void UnRegister (IEntity entity);
        void Deal (IEntity entity, string actionName, Hashtable args);
    }
}