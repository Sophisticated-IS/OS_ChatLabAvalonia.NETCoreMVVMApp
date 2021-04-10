

using System.Net;
using System.Reactive;
using OS_ChatLabAvalonia.NETCoreMVVMApp.Services;
using ReactiveUI;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly string _userName;
        private readonly string _userToken;
        private readonly IPEndPoint _serverEndPoint;
        private readonly ServerMessagesListener _serverMessagesListener;
        public ReactiveCommand<Unit, Unit> SendTextMessageCommand { get; }
        public string ChatMessageText { get; set; }
        public MainWindowViewModel(string userName,string userToken,IPEndPoint serverEndPoint)
        {
            _userName = userName;
            _userToken = userToken;
            _serverEndPoint = serverEndPoint;
            _serverMessagesListener = new ServerMessagesListener();
            SendTextMessageCommand = ReactiveCommand.Create(SendTextMessage);
        }

        private void SendTextMessage()
        {
            
        }
    }
}