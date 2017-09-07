using Client.Library.Entity;

namespace Assets.Scripts.FrameSync.SimulateServer {
    public class SimulateLocalServer : EntityBase {
        protected override int ThisType {
            get { return EntityConsts.EntityType.SimulateLocalServer; }
        }
    }
}