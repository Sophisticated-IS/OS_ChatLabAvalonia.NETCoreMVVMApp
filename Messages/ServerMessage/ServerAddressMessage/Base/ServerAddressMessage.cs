using ProtoBuf;

namespace Messages.ServerMessage.ServerAddressMessage.Base
{
    [ProtoContract]
    [ProtoInclude(10, typeof(FilesServerAddressMessage))]
    [ProtoInclude(11, typeof(TextServerAddressMessage))]
    public abstract class ServerAddressMessage : ServerMessage.Base.ServerMessage
    {
        [ProtoMember(1)]
        public string ServerIP { get; set; }
        [ProtoMember(2)]
        public int ServerPort { get; set; }
    }
}