using ProtoBuf;

namespace Messages.ServerMessage
{
    [ProtoContract]
    public sealed class NewUserConnected : Base.ServerMessage
    {
        [ProtoMember(1)]
        public string UserName { get; set; }
    }
}