using Messages.Base;
using ProtoBuf;

namespace Messages.ServerMessage.Base
{
    [ProtoContract]
    [ProtoInclude(8, typeof(ServerAddressMessage.Base.ServerAddressMessage))]
    [ProtoInclude(9, typeof(ServerFailMessage))]
    [ProtoInclude(10, typeof(ServerSuccessMessage))]
    public abstract class ServerMessage : Message
    {
        
    }
}