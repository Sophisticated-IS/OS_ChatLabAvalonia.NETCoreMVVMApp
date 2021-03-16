﻿using System;
using System.Net;
using System.Threading.Tasks;
using Avalonia.Media;
using OS_ChatLabAvalonia.NETCoreMVVMApp.Services;
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

        private UdpClientSender _udpClientSender;
        private TcpClientSender _tcpClientSender;
        private StatusConnection _connectionStatus;
        private ISolidColorBrush _connectionStatusColor;
        private bool _isWindowEnabled;


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
            ConnectToServer();
            // _tcpClientSender.Run();
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
                IsWindowEnabled = true;
            }
        }
    }
}