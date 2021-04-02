using ProtoBuf;

namespace Messages.ClientMessage.AuthorizedClientMessage
{
    [ProtoContract]
    public sealed class ClientReceivedFilePartMessage : Base.AuthorizedClientMessage
    {
        [ProtoMember(1)]
        public string FileToken { get; set; }
    }
}