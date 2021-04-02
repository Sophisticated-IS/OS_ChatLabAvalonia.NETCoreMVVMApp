using ProtoBuf;

namespace Messages.ClientMessage.UnauthorizedClientMessage.Base
{
    [ProtoContract]
    [ProtoInclude(7, typeof(RegisterClientMessage))]
    [ProtoInclude(8, typeof(WhoIsServerMessage))]
    public abstract class UnauthorizedClientMessage : ClientMessage.Base.ClientMessage
    {
        
    }
}