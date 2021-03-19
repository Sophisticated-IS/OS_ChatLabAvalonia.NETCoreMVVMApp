using ProtoBuf;

namespace Messages.ServerMessage
{
    [ProtoContract]
    public sealed class ServerFailMessage : Base.ServerMessage
    {
        [ProtoMember(1)]
        public int ErrorCode { get; set; }
        [ProtoMember(2)]
        public string ErrorMessage { get; set; }
    }
}