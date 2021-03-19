namespace Messages.ClientMessage.AuthorizedUserMessages.Base
{
    public abstract class AuthorizedUserMessage : ClientMessage.Base.ClientMessage
    {
        public string UserName { get; set; }
    }
}