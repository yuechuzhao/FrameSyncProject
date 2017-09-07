using System;
using System.Collections;
using System.Collections.Generic;

namespace Client.Library.Entity {

    /// <summary>
    /// 消息分发规则
    /// </summary>
    public class MessageDispatchRule {
        public int SenderType = EntityBase.WildEntityType;
        public string Message;
        public int[] ReceiverTypes;

        public bool CheckCanDispatch(int senderType, string message) {
            if (message != Message) return false;
            if (SenderType == EntityBase.WildEntityType) return true;
            if (SenderType == senderType) return true;
            return false;
        }
    }

    public abstract class BattleProcessorBase {

        protected Func<int, int, IEntity> GetEntityByID;

        /// <summary>
        /// 一开始考虑在构造函数当中使用虚函数构造，但是发现那样的话其实不会调用子类的方法（在构造函数时子类尚未构造）
        /// 因此采用了lazyLoad
        /// </summary>
        private List<MessageDispatchRule> _sendReceiveRules;
        private List<MessageDispatchRule> SendReceiveRules {
            get {
                if (_sendReceiveRules == null) {
                    _sendReceiveRules = new List<MessageDispatchRule>();
                    InitMessageDispatchRules();
                }
                return _sendReceiveRules;
            }
        }

        /// <summary>
        /// 一开始考虑在构造函数当中使用虚函数构造，但是发现那样的话其实不会调用子类的方法（在构造函数时子类尚未构造）
        /// 因此采用了lazyLoad
        /// </summary>
        private Dictionary<string, System.Func<Hashtable, Hashtable>> _argsProcessFunctions;
        private Dictionary<string, System.Func<Hashtable, Hashtable>> ArgsProcessFunctionss {
            get {
                if (_argsProcessFunctions == null) {
                    _argsProcessFunctions = new Dictionary<string, Func<Hashtable, Hashtable>>();
                    InitArgumentsProcessFunctions();
                }
                return _argsProcessFunctions;
            }
        }

        protected BattleProcessorBase(EntityPool pool) {
            pool.AddTrigger(OnTrigger);
            GetEntityByID = pool.GetTargetById;
        }

        protected virtual void InitArgumentsProcessFunctions() {
        }

        /// <summary>
        /// 初始化这个processor当中的消息转发规则
        /// </summary>
        protected virtual void InitMessageDispatchRules() {
        }

        protected void AddMessageDispatchRule(MessageDispatchRule rule) {
            _sendReceiveRules.Add(rule);
        }

        /// <summary>
        /// 挂载这个processor当中的参数处理方法
        /// </summary>
        /// <param name="message"></param>
        /// <param name="function"></param>
        protected void AddArgumentProcessFunction(string message,
            System.Func<Hashtable, Hashtable> function) {
            _argsProcessFunctions.Add(message, function);
        }


        protected bool OnTrigger(IEntity sender, string message, EntityPool.TriggerFunction callback,
            Hashtable args) {
            int totalRules = SendReceiveRules.Count;
            MessageDispatchRule availableRule = null;
            for (int index = 0; index < totalRules; index++) {
                var rule = SendReceiveRules[index];
                if (rule.CheckCanDispatch(sender.EntityType(), message)) {
                    availableRule = rule;
                    break;
                }
            }
            if (availableRule == null) return false;
            int[] receiverTypes = availableRule.ReceiverTypes;
            if (receiverTypes == null) return false;
            for (int rIndex = 0; rIndex < receiverTypes.Length; rIndex++) {
                int receiveType = receiverTypes[rIndex];
                // 参数可能会经由一个中间处理
                args = ProcessMessageArguments(availableRule.Message, args);
                callback(receiveType, message, args);
            } 
            return true;
        }

        /// <summary>
        /// 转发前可能需要对参数进行处理，如伤害计算等等
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected Hashtable ProcessMessageArguments(string message, Hashtable args) {
            System.Func<Hashtable, Hashtable> func;
            if (ArgsProcessFunctionss.TryGetValue(message, out func)) {
                return func(args);
            }
            return args;
        }
    }

}