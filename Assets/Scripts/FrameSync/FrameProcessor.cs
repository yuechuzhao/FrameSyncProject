using Client.Library.Entity;

namespace Assets.Scripts.FrameSync {
    public class FrameProcessor : BattleProcessorBase {
        public FrameProcessor(EntityPool pool) : base(pool) {
        }

        protected override void InitMessageDispatchRules() {
            AddMessageDispatchRule(new MessageDispatchRule() {
                SenderType = EntityConsts.EntityType.UnitManager,
                Message = EntityConsts.Message.JOIN_GAME,
                ReceiverTypes = new []{EntityConsts.EntityType.FrameController}
            });

            AddMessageDispatchRule(new MessageDispatchRule() {
                SenderType = EntityConsts.EntityType.InputController,
                Message = EntityConsts.Message.PLAY_OPERATION,
                ReceiverTypes = new []{EntityConsts.EntityType.FrameController}
            });
            
            AddMessageDispatchRule(new MessageDispatchRule() {
                SenderType = EntityConsts.EntityType.FrameController,
                Message = EntityConsts.Message.CREATE_UNIT,
                ReceiverTypes = new []{EntityConsts.EntityType.UnitManager}
            });
            
            AddMessageDispatchRule(new MessageDispatchRule() {
                SenderType = EntityConsts.EntityType.FrameController,
                Message = EntityConsts.Message.UNIT_MOVE,
                ReceiverTypes = new []{EntityConsts.EntityType.Unit}
            });
            
            
        }
    }
}