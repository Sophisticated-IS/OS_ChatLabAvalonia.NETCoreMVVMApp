using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Messages.ClientMessage.UnauthorizedClientMessage;
using Messages.ServerMessage;
using OS_ChatLabAvalonia.NETCoreMVVMApp.Views;
using ReactiveUI;
using Utils;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.ViewModels
{
    public sealed class RegisterUserWindowVM : ViewModelBase
    {
        public enum StatusConnection
        {
            Disconnected,
            Connected
        }

        public const string BroadcastIpAddress = "127.255.255.255"; //"10.255.255.255";
        private bool _isUserRegistered;
        private ISolidColorBrush _connectionStatusColor;
        private StatusConnection _connectionStatus;
        private bool _isProgressBarVisible;
        private IPEndPoint _serverEndPoint;
        private string _clientToken;
        private string[] _usersInChat;

        public string UserName { get; set; }


        public ISolidColorBrush ConnectionStatusColor
        {
            get => _connectionStatusColor;
            set => this.RaiseAndSetIfChanged(ref _connectionStatusColor, value);
        }

        public StatusConnection ConnectionStatus
        {
            get => _connectionStatus;
            set => this.RaiseAndSetIfChanged(ref _connectionStatus, value);
        }

        public bool IsProgressBarVisible
        {
            get => _isProgressBarVisible;
            set => this.RaiseAndSetIfChanged(ref _isProgressBarVisible, value);
        }

        public RegisterUserWindowVM()
        {
            this.WhenAnyValue(vm => vm.ConnectionStatus).Subscribe(x => UpdateConnectionStatusColor());
            FindServerAsync();
        }

        public void On_WindowClosing()
        {
            //Если пользователь отказался регистрироваться, то выходим 
            if (!_isUserRegistered) return;

            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var desktopMainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(UserName,_clientToken,_usersInChat,_serverEndPoint)
                };
                desktop.MainWindow = desktopMainWindow;
                desktopMainWindow.Show();
            }
        }

        private void UpdateConnectionStatusColor()
        {
            switch (ConnectionStatus)
            {
                case StatusConnection.Disconnected:
                    IsProgressBarVisible = true;
                    ConnectionStatusColor = Brushes.Red;
                    break;
                case StatusConnection.Connected:
                    IsProgressBarVisible = false;
                    ConnectionStatusColor = Brushes.Green;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void FindServerAsync()
        {
            using var udpClient = new UdpClient
            {
                EnableBroadcast = true
            };
            var whoIsServerMessage = new WhoIsServerMessage();
            var sendingMessageBytes = MessageConverter.PackMessage(whoIsServerMessage);

            var serverPort = 12300;
            bool isServerFound = false;
            while (!isServerFound)
            {
                var broadcastIpEndPoint = new IPEndPoint(IPAddress.Parse(BroadcastIpAddress), serverPort);
                try
                {
                    await udpClient.SendAsync(sendingMessageBytes, sendingMessageBytes.Length, broadcastIpEndPoint);
                }
                catch (Exception)
                {
                    // ignored
                }

                for (var i = 0; i < 2; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    if (udpClient.Available > 0)
                    {
                        var receivedResult = await udpClient.ReceiveAsync();
                        isServerFound = await UnpackServerAnswer(receivedResult.Buffer);
                    }
                }
            }
            
            ConnectionStatus = StatusConnection.Connected;
        }

        private Task<bool> UnpackServerAnswer(byte[] buffer)
        {
            Task<bool> isServerFound = Task.FromResult(false);
            try
            {
                var serverMessage = MessageConverter.UnPackMessage(buffer);
                switch (serverMessage)
                {
                    case ServerAddressMessage serverAddress:
                        isServerFound = Task.FromResult(true);
                        _serverEndPoint = new IPEndPoint(IPAddress.Parse(serverAddress.Ip), (int) serverAddress.Port);
                        break;
                }
            }
            catch (Exception)
            {
                isServerFound = Task.FromResult(false);
            }

            return isServerFound;
        }

        public void TryRegisterCommand()
        {
            if (ConnectionStatus != StatusConnection.Connected) return;
            if (string.IsNullOrEmpty(UserName)) return;
            
            using var tcpClient = new TcpClient();
            var registerRequest = new RegisterClientMessage
            {
                UserName = UserName
            };
            var packedMessage = MessageConverter.PackMessage(registerRequest);
            bool isServerConnected = TryConnectServer(tcpClient);
            if (isServerConnected)
            {
                var sendMessageResult = TrySendDataToServer(tcpClient, packedMessage);
                if (sendMessageResult)
                {
                    var buffer = new byte[1024];
                    var bytesAmount = tcpClient.Client.Receive(buffer);
                    var dataReceived = buffer.Take(bytesAmount).ToArray();
                    var message = MessageConverter.UnPackMessage(dataReceived);
                    if (message is ClientRegisteredMessage clientRegisteredMessage)
                    {
                        _clientToken = clientRegisteredMessage.ClientToken;
                        _isUserRegistered = true;
                        _usersInChat = clientRegisteredMessage.CurrentUsersListInChat;
                        tcpClient.Close();
                        CloseCurrentWindow();
                    }
                    else ;//ignored
                }
                else ;//ignored
            }
            else ;//не удалось установить соединение
        }

        private static void CloseCurrentWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow.Close();
            }
        }

        private bool TrySendDataToServer(TcpClient tcpClient, byte[] packedMessage)
        {
            var sendDataResult = false;
            try
            {
                tcpClient.Client.Send(packedMessage);
                sendDataResult = true;
            }
            catch (Exception)
            {
                // ignored
            }

            return sendDataResult;
        }

        private bool TryConnectServer(TcpClient tcpClient)
        {
            var connectionResult = false;
            try
            {
                tcpClient.Connect(_serverEndPoint);
                connectionResult = true;
            }
            catch (Exception)
            {
                // ignored
            }

            return connectionResult;
        }
    }
}