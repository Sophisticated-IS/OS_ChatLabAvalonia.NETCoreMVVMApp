using Messages.TftpServerMessages.Base;
using ProtoBuf;

namespace Messages.TftpServerMessages
{
    [ProtoContract]
    public class StartLoadFileMessage : TftpMessage
    {
        [ProtoMember(1)]
        public string FileName { get; set; }
    }
}