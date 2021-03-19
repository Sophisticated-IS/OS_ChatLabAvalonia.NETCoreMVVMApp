namespace Messages.ClientMessage.NotAuthorizedUserMessages
{
    public sealed class RegisterNewUserMessage : Base.ClientMessage
    {
        public string NewUserName { get; set; }
    }
}