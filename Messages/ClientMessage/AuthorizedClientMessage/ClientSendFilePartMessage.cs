using ProtoBuf;

namespace Messages.ClientMessage.AuthorizedClientMessage
{
    [ProtoContract]
    public sealed class ClientSendFilePartMessage : Base.AuthorizedClientMessage
    {
        [ProtoMember(1)]
        public byte[] Data { get; set; }
        [ProtoMember(2)]
        public string FileName { get; set; }
    }
}