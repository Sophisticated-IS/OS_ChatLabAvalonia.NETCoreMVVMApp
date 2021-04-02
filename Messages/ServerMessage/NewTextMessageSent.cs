using ProtoBuf;

namespace Messages.ServerMessage
{
    [ProtoContract]
    public sealed class NewTextMessageSent : Base.ServerMessage
    {
        [ProtoMember(1)]
        public string UserName { get; set; }
        [ProtoMember(2)]
        public string TextMessage { get; set; }
    }
}