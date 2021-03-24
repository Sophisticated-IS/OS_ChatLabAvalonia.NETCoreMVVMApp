using Messages.TftpServerMessages.Base;
using ProtoBuf;

namespace Messages.TftpServerMessages
{
    [ProtoContract]
    public sealed class AskForSendFilePartMessage : TftpMessage
    {
    }
}