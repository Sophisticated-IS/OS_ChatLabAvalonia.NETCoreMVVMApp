using ProtoBuf;

namespace Messages.ClientMessage.UnauthorizedClientMessage
{
    [ProtoContract]
    public sealed class RegisterClientMessage : Base.UnauthorizedClientMessage
    {
        [ProtoMember(1)]
        public string UserName { get; set; }
    }
}