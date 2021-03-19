using ProtoBuf;

namespace Messages.ClientMessage.NotAuthorizedUserMessages
{
    [ProtoContract]
    public sealed class RegisterNewUserMessage : Base.ClientMessage
    {
        [ProtoMember(1)]
        public string NewUserName { get; set; }
    }
}