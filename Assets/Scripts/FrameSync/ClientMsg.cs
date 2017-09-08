
namespace Assets.Scripts.FrameSync
{

    public class ClientMsg
    {
        public const int CREATION = 1;
        public const int MOVE = 2;

        public const char FIRST_SPLITER = '|';
        public const char SECOND_SPLITER = ';';

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
            return string.Join(SECOND_SPLITER.ToString(), new []{
                FrameId.ToString(),
                Guid, OperationCode.ToString(),
                InfoToString(OperationInfo)});
        }

        protected virtual string InfoToString(object info){
            return info == null ? "" : info.ToString();
        }

        public virtual object ParseInfo() {
            return OperationInfo.ToString();
        }

        public static ClientMsg ParseFrom(string source)
        {
            string[] array = source.Split(SECOND_SPLITER);
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
        Forward = 1,
        Back = 2,
        Left = 3,
        Right = 4,
    }

    public static class UnitMoveInfo {

        public static string ParseToProtoString(this string actionString) {
            return actionString.ToString();
        }
    }

    public class UnitMoveMsg : ClientMsg {
        public UnitMoveMsg() : base()
        {
            OperationCode = MOVE;
        }
    }
    
}