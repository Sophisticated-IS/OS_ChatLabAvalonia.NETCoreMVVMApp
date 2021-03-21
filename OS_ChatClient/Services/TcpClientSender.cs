using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Messages.Base;
using Messages.ClientMessage.AuthorizedUserMessages;
using Messages.ClientMessage.NotAuthorizedUserMessages;
using Messages.ServerMessage;
using OS_ChatLabAvalonia.NETCoreMVVMApp.Models;
using Utils;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Services
{
    public sealed class TcpClientSender
    {
        private readonly ObservableCollection<ChatMessage> _chatMessages;
        private readonly ObservableCollection<string> _usersInChat;
        public string TcpClientIP;
        public int TcpClientPort;

        private Socket _tcpSocket;

        private static List<string> TcpIPs = new List<string>(){"127.0.0.98","127.0.0.99"}; 
        private static List<int> TcpPorts = new List<int>(){8084,8094};

        public TcpClientSender(IPEndPoint serverIpEndPoint, [NotNull] ObservableCollection<ChatMessage> chatMessages,
            [NotNull] ObservableCollection<string> usersInChat)
        {
            _chatMessages = chatMessages ?? throw new ArgumentNullException(nameof(chatMessages));
            _usersInChat = usersInChat ?? throw new ArgumentNullException(nameof(usersInChat));

            var Mutex = new Mutex(true, "EB617FE4-1610-4FE9-82B7-853EC0D77F24");

            if (Mutex.WaitOne(TimeSpan.Zero))
            {
                TcpClientIP = TcpIPs.First();
                TcpClientPort = TcpPorts.First();
            }
            else
            {
                TcpClientIP = TcpIPs.Last();
                TcpClientPort = TcpPorts.Last();
            }
            
            SetConnection(serverIpEndPoint);
        }

        public async void Run()
        {
            while (true)
            {
                var fullMessage = new List<byte>(1024);
                do
                {
                    var buffer = new byte[1024];
                    var bytesAmount =await _tcpSocket.ReceiveAsync(buffer, SocketFlags.None);
                    var realGotBytes = buffer.Take(bytesAmount);
                    fullMessage.AddRange(realGotBytes);
                } while (_tcpSocket.Available > 0);

                var receivedMessage = MessageConverter.UnPackMessage(fullMessage.ToArray());

                switch (receivedMessage)
                {
                    case SendTextMessage sendTextMessage:
                        if (!string.IsNullOrEmpty(sendTextMessage.TextMessage))
                        {
                            _chatMessages.Add(new ChatMessage(sendTextMessage.UserName, sendTextMessage.TextMessage,
                                DateTime.Now));
                            if (!_usersInChat.Contains(sendTextMessage.UserName))
                            {
                                _usersInChat.Add(sendTextMessage.UserName);
                            }
                        }
                        break;
                }
            }

        }


        private bool SetConnection(IPEndPoint serverIpEndPoint)
        {
            try
            {
                var ipEndPoint = new IPEndPoint(IPAddress.Parse(TcpClientIP), TcpClientPort);

                _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _tcpSocket.Bind(ipEndPoint);

                _tcpSocket.Connect(serverIpEndPoint);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async void SendMessage(Message message)
        {
            var sendingMessage = MessageConverter.PackMessage(message);
            await _tcpSocket.SendAsync(sendingMessage, SocketFlags.None);
        }

        public async Task<bool> RegisterUser(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            var registerNewUserMessage = new RegisterNewUserMessage {NewUserName = name};
            var sendingMessage = MessageConverter.PackMessage(registerNewUserMessage);
            await _tcpSocket.SendAsync(sendingMessage, SocketFlags.None);

            var data = new byte[1024];
            var bytesAmount = await _tcpSocket.ReceiveAsync(data, SocketFlags.None);

            var answerMessage = data.Take(bytesAmount).ToArray();
            var resultMessage = MessageConverter.UnPackMessage(answerMessage);

            switch (resultMessage)
            {
                case ServerFailMessage serverFailMessage:
                    return false;

                case ServerSuccessMessage serverSuccessMessage:
                    return true;

                default: return false;
            }
        }

        ~TcpClientSender()
        {
            _tcpSocket?.Disconnect(false);
            _tcpSocket?.Close();
        }
    }
}