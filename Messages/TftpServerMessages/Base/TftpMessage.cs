using ProtoBuf;

namespace Messages.TftpServerMessages.Base
{
    [ProtoContract]
    [ProtoInclude(120,typeof(ReceivePartFileMessage))]
    [ProtoInclude(130,typeof(StartLoadFileMessage))]
    [ProtoInclude(140,typeof(EndLoadFileMessage))]
    [ProtoInclude(150,typeof(AskForStartSendingFileMessage))]
    [ProtoInclude(160,typeof(AskForSendFilePartMessage))]
    public abstract class TftpMessage : IMessageProvider
    {
    }
}