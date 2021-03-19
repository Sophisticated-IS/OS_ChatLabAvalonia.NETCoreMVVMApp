namespace Messages.ServerMessage
{
    public sealed class ServerFailMessage : Base.ServerMessage
    {
        public int ErrorCode { get; set; }  
        public string ErrorMessage { get; set; }
    }
}