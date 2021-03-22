using ProtoBuf;

namespace Messages.TftpServerMessages.Base
{
    [ProtoContract]
    [ProtoInclude(120,typeof(SendPartFileMessage))]
    [ProtoInclude(130,typeof(StartLoadFileMessage))]
    public abstract class TftpMessage : IMessageProvider
    {
        
    }
}