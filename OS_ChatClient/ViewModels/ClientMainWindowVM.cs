using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Messages.ClientMessage.AuthorizedUserMessages;
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
        private TcpClientSender _tcpClientSender;
        private TftpClientSender _tftpClientSender;
        private StatusConnection _connectionStatus;
        private ISolidColorBrush _connectionStatusColor;
        private bool _isWindowEnabled;
        private string _userName;

        public ObservableCollection<ChatMessage> ChatMessages { get; }
        public ObservableCollection<string> UsersInChat { get; }
        public string ChatMessageText { get; set; }
        public ReactiveCommand<Unit, Unit> SendMessageCommand { get; }
        public ReactiveCommand<string, Unit> LoadFileCommand { get; }
        public ReactiveCommand<Unit, Unit> SendFileCommand { get; }

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
            LoadFileCommand = ReactiveCommand.Create<string>(LoadFile);
            SendFileCommand = ReactiveCommand.Create(SendFile);
            ConnectToServer();
            ChatMessages = new ObservableCollection<ChatMessage>();
            UsersInChat = new ObservableCollection<string>();
        }

        private async void GetTftpServerInformation()
        {
            var (ip, port) = await _tcpClientSender.GetTftpServerAddress();
            var ipAddress = IPAddress.Parse(ip);
            var ipEndPoint = new IPEndPoint(ipAddress,port);
            _tftpClientSender = new TftpClientSender(ipEndPoint);
        }

        private async void SendFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.AllowMultiple = false;
            string filePath = null;
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var result = await openFileDialog.ShowAsync(desktop.MainWindow);
                if (result.Length == 0) return;
                filePath = result.First();
            }

            var fileName = Path.GetFileName(filePath);

            var msg = new ChatMessage(_userName, fileName, DateTime.Now, true);
            ChatMessages.Add(msg);
            await _tftpClientSender.SendFile(filePath);
        }

        private void LoadFile(string fileNameParameter)
        {
        }

        private void SendMessage()
        {
            var mess = new SendTextMessage {TextMessage = ChatMessageText, UserName = _userName};
            _tcpClientSender.SendMessage(mess);
            ChatMessages.Add(new ChatMessage(_userName, ChatMessageText, DateTime.Now, false));
        }


        private async void ConnectToServer()
        {
            var resultIpEndPoint = await _udpClientSender.FindServer();
            //искуственная задержка
            await Task.Delay(TimeSpan.FromSeconds(4));

            if (resultIpEndPoint != null)
            {
                _tcpClientSender = new TcpClientSender(resultIpEndPoint, ChatMessages, UsersInChat);
                ConnectionStatus = StatusConnection.Connected;
                ConnectionStatusColor = Brushes.Lime;

                var registerDialog = new UserNameDialog();
                registerDialog.SetDataContextWithArgs(_tcpClientSender);

                if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    await registerDialog.ShowDialog(desktop.MainWindow);
                }

                var dataContext = (UserNameDialogVM) registerDialog.DataContext;
                _userName = dataContext.UserName;
                UsersInChat.Add(_userName + "(YOU)");
                GetTftpServerInformation();
                IsWindowEnabled = true;
                _tcpClientSender.Run();
            }
        }
    }
}