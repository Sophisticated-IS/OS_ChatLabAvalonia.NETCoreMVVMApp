using ProtoBuf;

namespace Messages.ServerMessage
{
    [ProtoContract]
    public sealed class ClientRegisteredMessage : Base.ServerMessage
    {
        [ProtoMember(1)]
        public string ClientToken { get; set; }
        [ProtoMember(2)] 
        public string[] CurrentUsersListInChat { get; set; }
    }
}