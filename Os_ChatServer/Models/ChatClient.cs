
namespace Os_ChatServer.Models
{
    public sealed class ChatClient : User
    {
        public string IP { get; set; }
        public int Port { get; set; }
    }
}