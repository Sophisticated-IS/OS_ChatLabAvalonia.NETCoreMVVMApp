using ProtoBuf;

namespace Messages.ClientMessage.AuthorizedUserMessages.Base
{
    [ProtoContract]
    [ProtoInclude(6, typeof(GetFilesServerAddressMessage))]
    [ProtoInclude(7, typeof(SendTextMessage))]
    public abstract class AuthorizedUserMessage : ClientMessage.Base.ClientMessage
    {
        [ProtoMember(1)]
        public string UserName { get; set; }
    }
}