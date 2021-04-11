using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Messages.Base;
using Messages.ServerMessage;
using Utils;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Services
{
    public class ServerMessagesListener
    {
        private readonly EndPoint _serverEndPoint;
        private Socket? _connection;
        public const string BaseIpAddress = "127.0.0.";
        private int _port;
        private string _ipAddress;


        public delegate void UserConnectedHandler(NewUserConnected newUserConnected);
        public event UserConnectedHandler NewUserConnectedEvent;

        public delegate void TextMessageSentHandler(NewTextMessageSent newTextMessageSent);
        public event TextMessageSentHandler TextMessageSentEvent;

        
        public ServerMessagesListener(EndPoint serverEndPoint)
        {
            _serverEndPoint = serverEndPoint;
            _port = PortProvider.GetAvailablePort();
            _ipAddress = GetAvailableIp();
            ThreadPool.QueueUserWorkItem(StartListenForServerMessages);
        }

        private string GetAvailableIp()
        {
            string fullAddress = string.Empty;
            for (int i = 5; i < 255; i++)
            {
                fullAddress = BaseIpAddress + i;
                try
                {
                    using var tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    EndPoint endPoint = new IPEndPoint(IPAddress.Parse(fullAddress), _port);
                    tcpClient.Bind(endPoint);
                    return fullAddress;
                }
                catch (Exception e)
                {
                    continue;
                }
            }

            throw new ArgumentOutOfRangeException();
        }
        
        private async void StartListenForServerMessages(object? state)
        {
            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
            tcpSocket.Bind(ipEndPoint);
            tcpSocket.Connect(_serverEndPoint);
            _connection = tcpSocket;
            
            while (true)
            {
                var fullData = new List<byte>(1024);
                EndPoint clientIpEndPoint = new IPEndPoint(IPAddress.None, 0);
                do
                {
                    var buffer = new byte[128];
                    var receiveFromAsync = await tcpSocket.ReceiveFromAsync(buffer, SocketFlags.None, clientIpEndPoint);
                    var receivedBytes = buffer.Take(receiveFromAsync.ReceivedBytes);
                    clientIpEndPoint = receiveFromAsync.RemoteEndPoint;
                    fullData.AddRange(receivedBytes);
                } while (tcpSocket.Available > 0);

                var receivedMessage = TryUnpackMessage(fullData.ToArray());
                if (receivedMessage != null)
                {
                    switch (receivedMessage)
                    {
                        case NewUserConnected newUserConnected:    
                            NewUserConnectedEvent?.Invoke(newUserConnected);
                            break;
                        case NewTextMessageSent newTextMessageSent:
                            TextMessageSentEvent?.Invoke(newTextMessageSent);
                            break;
                    }
                }
            }
        }
        private Message? TryUnpackMessage(in byte[] data)
        {
            try
            {
                var unpackedMessage = MessageConverter.UnPackMessage(data);
                return unpackedMessage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Task<bool> SendMessage(Message message)
        {
            if (_connection is null) return Task.FromResult(false);

            var result = false;
            try
            {
                var packedMessage = MessageConverter.PackMessage(message);
                _connection.Send(packedMessage);
                result = true;
            }
            catch (Exception)
            {
                //ignored
            }

            return Task.FromResult(result);
        }
        
    }
}