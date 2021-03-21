using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Messages.ClientMessage.AuthorizedUserMessages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OS_ChatLabAvalonia.NETCoreMVVMApp.Models;
using OS_ChatLabAvalonia.NETCoreMVVMApp.Services;
using OS_ChatLabAvalonia.NETCoreMVVMApp.Views;
using ReactiveUI;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public enum StatusConnection
        {
            Disconnected,
            Reconnecting,
            Connected
        }

        private readonly UdpClientSender _udpClientSender;
        private  TcpClientSender _tcpClientSender;
        private StatusConnection _connectionStatus;
        private ISolidColorBrush _connectionStatusColor;
        private bool _isWindowEnabled;
        private string _userName;

        public ObservableCollection<ChatMessage> ChatMessages { get;}
        public ObservableCollection<string> UsersInChat { get;}
        public string ChatMessageText { get; set; }
        public ReactiveCommand<Unit,Unit> SendMessageCommand { get;  }
        public ReactiveCommand<Unit,Unit> SendFileCommand { get; set; }
        public StatusConnection ConnectionStatus
        {
            get => _connectionStatus;
            set => this.RaiseAndSetIfChanged(ref _connectionStatus, value);
        }

        public ISolidColorBrush ConnectionStatusColor
        {
            get => _connectionStatusColor;
            set => this.RaiseAndSetIfChanged(ref _connectionStatusColor, value);
        }

        public bool IsWindowEnabled
        {
            get => _isWindowEnabled;
            set => this.RaiseAndSetIfChanged(ref _isWindowEnabled, value);
        }

        public MainWindowViewModel()
        {
            ConnectionStatus = StatusConnection.Disconnected;
            ConnectionStatusColor = Brushes.Red;
            _udpClientSender = new UdpClientSender();
            SendMessageCommand = ReactiveCommand.Create(SendMessage);
            ConnectToServer();
            ChatMessages = new ObservableCollection<ChatMessage>();
            UsersInChat = new ObservableCollection<string>();
        }

        private void SendMessage()
        {
            var mess = new SendTextMessage {TextMessage = ChatMessageText};
            _tcpClientSender.SendMessage(mess);
            ChatMessages.Add(new ChatMessage(_userName,ChatMessageText,DateTime.Now));
        }

        
        
        private async void ConnectToServer()
        {
            var resultIpEndPoint = await _udpClientSender.FindServer();
            //искуственная задержка
            await Task.Delay(TimeSpan.FromSeconds(4));

            if (resultIpEndPoint != null)
            {
                _tcpClientSender = new TcpClientSender(resultIpEndPoint);
                ConnectionStatus = StatusConnection.Connected;
                ConnectionStatusColor = Brushes.Lime;

                var registerDialog = new UserNameDialog();
                registerDialog.SetDataContextWithArgs(_tcpClientSender);
                
                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    await registerDialog.ShowDialog(desktop.MainWindow);
                }

                var dataContext = (UserNameDialogVM)registerDialog.DataContext;
                _userName = dataContext.UserName;
                UsersInChat.Add(_userName + "(YOU)");
                IsWindowEnabled = true;
                
            }
        }
    }
}