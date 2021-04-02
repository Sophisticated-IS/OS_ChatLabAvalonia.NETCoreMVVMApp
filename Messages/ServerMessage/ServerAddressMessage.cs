using ProtoBuf;

namespace Messages.ServerMessage
{
    [ProtoContract]
    public sealed class ServerAddressMessage : Base.ServerMessage
    {
        [ProtoMember(1)]
        public string Ip { get; set; }
        [ProtoMember(2)]
        public uint Port { get; set; }
    }
}