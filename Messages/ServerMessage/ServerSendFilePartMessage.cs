using ProtoBuf;

namespace Messages.ServerMessage
{
    [ProtoContract]
    public sealed class ServerSendFilePartMessage : Base.ServerMessage
    {
        [ProtoMember(1)]
        public byte[] Data { get; set; }

        [ProtoMember(2)]
        public string FileName { get; set; }
    }
}