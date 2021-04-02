using Messages.Base;
using ProtoBuf;

namespace Messages.ServerMessage.Base
{
    [ProtoContract]
    [ProtoInclude(9, typeof(ClientRegisteredMessage))]
    [ProtoInclude(10, typeof(ServerSendFilePartMessage))]
    [ProtoInclude(11, typeof(FileWasTransferredMessage))]
    [ProtoInclude(12, typeof(NewUserConnected))]
    [ProtoInclude(13, typeof(NewTextMessageSent))]
    [ProtoInclude(14, typeof(ServerAddressMessage))]
    [ProtoInclude(15, typeof(ServerFailMessage))]
    [ProtoInclude(16, typeof(ServerSuccessMessage))]
    public abstract class ServerMessage : Message
    {
        
    }
}