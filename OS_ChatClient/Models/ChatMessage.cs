using System;
using JetBrains.Annotations;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Models
{
    public class ChatMessage
    {
        public string UserName { get;}
        public string Message { get; }
        public DateTime Date { get; set; }

        public ChatMessage([NotNull] string userName, [NotNull] string message, DateTime date)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Date = date;
        }
    }
}