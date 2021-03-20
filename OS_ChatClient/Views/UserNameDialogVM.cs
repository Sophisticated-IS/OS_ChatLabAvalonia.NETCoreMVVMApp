using System.Reactive;
using Avalonia.Controls;
using OS_ChatLabAvalonia.NETCoreMVVMApp.Services;
using OS_ChatLabAvalonia.NETCoreMVVMApp.ViewModels;
using ReactiveUI;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Views
{
    public class UserNameDialogVM : ViewModelBase
    {
        private readonly Window _window;
        private TcpClientSender ClientSender { get; }

        public string UserName { get; set; }
        public ReactiveCommand<Unit,Unit> OKCommand { get; }

        public UserNameDialogVM(Window window, TcpClientSender clientSender)
        {
            _window = window;
            OKCommand = ReactiveCommand.Create(OK);
            ClientSender = clientSender;
        }
        
        private async void OK()
        {
            if (string.IsNullOrEmpty(UserName)) return;
            var result = await ClientSender.RegisterUser(UserName);
            
            if (result)
            {
               _window.Close();
            }
            
        }
    }
}