using ProtoBuf;

namespace Messages.ServerMessage
{
    [ProtoContract]
    public sealed class FileWasTransferredMessage : Base.ServerMessage
    {
        [ProtoMember(1)]
        public string FileToken { get; set; }
        [ProtoMember(2)]
        public string FileName { get; set; }

        [ProtoMember(3)]
        public string UserName { get; set; }
    }
}