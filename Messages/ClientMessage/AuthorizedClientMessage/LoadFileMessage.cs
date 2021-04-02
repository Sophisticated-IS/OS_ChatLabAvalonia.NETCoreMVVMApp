using ProtoBuf;

namespace Messages.ClientMessage.AuthorizedClientMessage
{
    [ProtoContract]
    public sealed class LoadFileMessage : Base.AuthorizedClientMessage
    {
        [ProtoMember(1)]
        public string FileToken { get; set; }
    }
}