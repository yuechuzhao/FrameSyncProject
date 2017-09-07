using System.Collections;

namespace Client.Library.Entity {
    public interface IEntity {
        int ObjectId { get; }
        int EntityType ();
        void Register ();
        void UnRegister ();
        bool IsEntity (int id);
        void Send (string actionName, Hashtable args);
        void Receive (string actionName, Hashtable args);
    }

}

