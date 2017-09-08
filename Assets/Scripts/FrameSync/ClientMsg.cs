
namespace Assets.Scripts.FrameSync
{

    public class ClientMsg
    {
        public const int CREATION = 1;
        public const int MOVE = 2;

        public int FrameId;//逻辑上第几帧
        public string Guid;//必须有guid，用来标识本机
        public int OperationCode;
        public int SendId = -1;//发送序号，默认等于-1，表示发送出去的
        public string OperationInfoStr;// 字符串
        public object OperationInfo;//其他操作信息，可以自定义序列化

        protected ClientMsg()
        {
        }

        public override string ToString() {
            return string.Join("|", new []{
                FrameId.ToString(),
                Guid, OperationCode.ToString(),
                InfoToString(OperationInfo)});
        }

        protected virtual string InfoToString(object info){
            return "";
        }

        public virtual object ParseInfo() {
            return OperationInfo.ToString();
        }

        public static ClientMsg ParseFrom(string source)
        {
            string[] array = source.Split('|');
            ClientMsg data = new ClientMsg(){
                FrameId = int.Parse(array[0]),
                Guid = array[1],
                OperationCode = int.Parse(array[2]),
                SendId = int.Parse(array[3]),
                OperationInfoStr = array[4]
                };
            return data;
        }
        
    }

    public class UnitCreateData {
        public int UnitId;
        public string Guid;
    }
    

    public class UnitCreationMsg : ClientMsg {
        public UnitCreationMsg() : base()
        {
            OperationCode = CREATION;
        }

        public override object ParseInfo() {
            return OperationInfoStr.ToString();//转换成uuid
        }
    }

    public enum EMoveActionType {
        Forward,
        Back,
        Left,
        Right,
    }

    public static class UnitMoveInfo {

        public static string ParseToProtoString(this string actionString) {
            EMoveActionType actionType = default(EMoveActionType);
            switch (actionString) {
                case "Forward":
                    actionType = EMoveActionType.Forward;
                    break;
                case "Back":
                    actionType = EMoveActionType.Back;
                    break;
                case "Left":
                    actionType = EMoveActionType.Left;
                    break;
                case "Right":
                    actionType = EMoveActionType.Right;
                    break;
            }
            return actionType.ToString();
        }
    }

    public class UnitMoveMsg : ClientMsg {
        public UnitMoveMsg() : base()
        {
            OperationCode = MOVE;
        }
    }
    
}