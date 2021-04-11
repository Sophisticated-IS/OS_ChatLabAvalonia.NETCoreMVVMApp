using System;
using JetBrains.Annotations;
using OS_ChatLabAvalonia.NETCoreMVVMApp.ViewModels;
using ReactiveUI;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Models
{
    public sealed class ChatMessage : ViewModelBase
    {
        private bool _isFileLoading;
        public string UserName { get;}
        public string Message { get; }
        public DateTime Date { get; }
        public bool IsFileMessage { get; }

        public bool IsFileLoading   
        {
            get => _isFileLoading;
            set => this.RaiseAndSetIfChanged(ref _isFileLoading,value);
        }

        public string FileToken { get; set; }

        public ChatMessage([NotNull] string userName, [NotNull] string message, DateTime date, bool isFileMessage)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Date = date;
            IsFileMessage = isFileMessage;
        }
    }
}