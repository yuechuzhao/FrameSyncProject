using Client.Library.Entity;

namespace Assets.Scripts.FrameSync {
    public class FrameProcessor : BattleProcessorBase {
        public FrameProcessor(EntityPool pool) : base(pool) {
        }

        protected override void InitMessageDispatchRules() {
            AddMessageDispatchRule(new MessageDispatchRule() {
                SenderType = EntityConsts.EntityType.Player,
                Message = EntityConsts.Message.UNIT_CREATED,
                ReceiverTypes = new []{EntityConsts.EntityType.FrameController}
            });

            AddMessageDispatchRule(new MessageDispatchRule() {
                SenderType = EntityConsts.EntityType.InputController,
                Message = EntityConsts.Message.PLAY_OPERATION,
                ReceiverTypes = new []{EntityConsts.EntityType.FrameController}
            });
            
            AddMessageDispatchRule(new MessageDispatchRule() {
                SenderType = EntityConsts.EntityType.FrameController,
                Message = EntityConsts.Message.ACTIVATE_SELF,
                ReceiverTypes = new []{EntityConsts.EntityType.Player}
            });
            
            
        }
    }
}