using ProtoBuf;

namespace Messages.TftpServerMessages.Base
{
    [ProtoContract]
    [ProtoInclude(120,typeof(SendPartFileMessage))]
    [ProtoInclude(130,typeof(StartLoadFileMessage))]
    [ProtoInclude(140,typeof(EndLoadFileMessage))]
    public abstract class TftpMessage : IMessageProvider
    {
        
    }
}