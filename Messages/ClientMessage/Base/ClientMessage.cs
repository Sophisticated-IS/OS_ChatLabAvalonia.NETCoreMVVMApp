using Messages.Base;
using ProtoBuf;

namespace Messages.ClientMessage.Base
{
    [ProtoContract]
    [ProtoInclude(1,typeof(AuthorizedClientMessage.Base.AuthorizedClientMessage)) ]
    [ProtoInclude(2,typeof(UnauthorizedClientMessage.Base.UnauthorizedClientMessage))]
    public abstract  class ClientMessage : Message
    {
        
    }
}