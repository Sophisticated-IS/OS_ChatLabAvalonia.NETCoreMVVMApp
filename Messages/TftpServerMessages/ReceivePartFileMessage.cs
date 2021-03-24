using Messages.TftpServerMessages.Base;
using ProtoBuf;

namespace Messages.TftpServerMessages
{
    [ProtoContract]
    public sealed class ReceivePartFileMessage : TftpMessage
    {
        [ProtoMember(1)]
        public  byte[] BytesData { get; set; }
    }
}