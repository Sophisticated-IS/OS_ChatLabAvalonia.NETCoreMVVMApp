using Messages.ClientMessage.AuthorizedUserMessages.Base;
using ProtoBuf;

namespace Messages.ClientMessage.AuthorizedUserMessages
{
    [ProtoContract]
    public sealed class SendTextMessage : AuthorizedUserMessage
    {
        [ProtoMember(1)]
        public string TextMessage { get; set; }
        
        [ProtoMember(2)]
        public bool IsFileNameMessage { get; set; }
    }
}