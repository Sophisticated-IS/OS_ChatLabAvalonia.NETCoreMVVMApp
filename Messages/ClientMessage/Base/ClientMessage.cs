using Messages.Base;
using Messages.ClientMessage.AuthorizedUserMessages.Base;
using Messages.ClientMessage.NotAuthorizedUserMessages;
using ProtoBuf;

namespace Messages.ClientMessage.Base
{
    [ProtoContract]
    [ProtoInclude(3, typeof(AuthorizedUserMessage))]
    [ProtoInclude(4, typeof(RegisterNewUserMessage))]
    [ProtoInclude(5, typeof(WhoIsServerMessage))]
    public abstract class ClientMessage : Message
    {
        
    }
}