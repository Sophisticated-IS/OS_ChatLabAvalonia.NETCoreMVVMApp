using Messages.ClientMessage.AuthorizedUserMessages.Base;
using ProtoBuf;

namespace Messages.ClientMessage.AuthorizedUserMessages
{
    [ProtoContract]
    public sealed class GetFilesServerAddressMessage : AuthorizedUserMessage
    {
    }
}