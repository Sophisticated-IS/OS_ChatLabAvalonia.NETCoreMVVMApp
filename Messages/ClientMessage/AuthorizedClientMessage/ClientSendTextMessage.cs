using ProtoBuf;

namespace Messages.ClientMessage.AuthorizedClientMessage
{
    [ProtoContract]
    public sealed class ClientSendTextMessage : Base.AuthorizedClientMessage
    {
        [ProtoMember(1)]
        public string TextMessage { get; set; }
    }
}