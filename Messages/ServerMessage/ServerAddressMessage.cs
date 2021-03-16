using ProtoBuf;

namespace Messages.ServerMessage
{
    [ProtoContract]
    public sealed class ServerAddressMessage : Base.ServerMessage
    {
        [ProtoMember(1)]
        public string ServerIP { get; set; }
    }
}