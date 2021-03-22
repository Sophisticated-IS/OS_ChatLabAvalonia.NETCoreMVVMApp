using ProtoBuf;

namespace Messages.TftpServerMessages.Base
{
    [ProtoContract]
    [ProtoInclude(1,typeof(SendPartFileMessage))]
    [ProtoInclude(2,typeof(SendPartFileMessage))]
    public abstract class TftpMessage : IMessageProvider
    {
        
    }
}