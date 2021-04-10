using ProtoBuf;

namespace Messages.ClientMessage.AuthorizedClientMessage.Base
{
    [ProtoContract]
    [ProtoInclude(3, typeof(ClientReceivedFilePartMessage))]
    [ProtoInclude(4, typeof(LoadFileMessage))]
    [ProtoInclude(5, typeof(ClientSendTextMessage))]
    [ProtoInclude(6, typeof(ClientSendFilePartMessage))]
    public abstract class AuthorizedClientMessage : ClientMessage.Base.ClientMessage
    {
        [ProtoMember(1)]
        public string ClientToken { get; set; }
    }
}