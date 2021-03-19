using Messages.Base;
using Messages.ServerMessage.ServerAddressMessage;
using ProtoBuf;

namespace Messages.ServerMessage.Base
{
    [ProtoContract]
    [ProtoInclude(4, typeof(TextServerAddressMessage))]
    public abstract class ServerMessage : Message
    {
        
    }
}