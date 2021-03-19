using Messages.ClientMessage.AuthorizedUserMessages.Base;

namespace Messages.ClientMessage.AuthorizedUserMessages
{
    public sealed class SendTextMessage : AuthorizedUserMessage
    {
        public string TextMessage { get; set; }
    }
}