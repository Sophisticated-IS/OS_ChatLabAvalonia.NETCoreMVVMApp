using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Messages.Base;
using Messages.ClientMessage.UnauthorizedClientMessage;
using Messages.ServerMessage;
using Utils;

namespace Os_ChatServer.Services
{
    public sealed class RegistrationClientService
    {
        private readonly string _tcpIp;
        private readonly int _tcpPort;
        private ConcurrentDictionary<EndPoint, string> _clientConnectionToken;
        private ConcurrentDictionary<EndPoint, Socket> _clientsConnections;

        public RegistrationClientService(string tcpIp, int tcpPort)
        {
            _clientsConnections = new ConcurrentDictionary<EndPoint, Socket>();
            _clientConnectionToken = new ConcurrentDictionary<EndPoint, string>();
            _tcpIp = tcpIp;
            _tcpPort = tcpPort;
            ThreadPool.QueueUserWorkItem(ClientConnectionsAcceptor);
        }

        private async void ClientConnectionsAcceptor(object? state)
        {
            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(_tcpIp), _tcpPort);
            tcpSocket.Bind(ipEndPoint);
            tcpSocket.Listen(100);
            var clientConnection = await tcpSocket.AcceptAsync();
            var isClientAdded = _clientsConnections.TryAdd(clientConnection.RemoteEndPoint, clientConnection);
            if (isClientAdded)
            {
                ThreadPool.QueueUserWorkItem(RunServerRegistration, clientConnection);
            }
        }

        private async void RunServerRegistration(object? state)
        {
            if (state is null) return;

            var clientSocket = (Socket) state;
            while (true)
            {
                var fullData = new List<byte>(1024);
                EndPoint clientIpEndPoint = new IPEndPoint(IPAddress.None, 0);
                do
                {
                    var buffer = new byte[128];
                    var data = await TryReceiveData(clientSocket, buffer, clientIpEndPoint);
                    //если не удается получить данные то скорей всего клиент отключился и можно перестать его обслуживать
                    if (data is null) return;

                    var receivedBytes = buffer.Take(data.Value);
                    fullData.AddRange(receivedBytes);
                } while (clientSocket.Available > 0);

                var receivedMessage = TryUnpackMessage(fullData.ToArray());
                if (receivedMessage != null)
                {
                    switch (receivedMessage)
                    {
                        case RegisterClientMessage registerClientMessage:
                            HandleRegisterClientMessage(clientSocket, registerClientMessage);
                            break;
                    }
                }
            }
        }

        private async Task<int?> TryReceiveData(Socket clientSocket, byte[] buffer, EndPoint clientIpEndPoint)
        {
            try
            {
                var result = await clientSocket.ReceiveFromAsync(buffer, SocketFlags.None, clientIpEndPoint);
                clientIpEndPoint = result.RemoteEndPoint;
                return result.ReceivedBytes;
            }
            catch (Exception)
            {
                _clientsConnections.Remove(clientSocket.RemoteEndPoint, out _);
                _clientConnectionToken.TryRemove(clientSocket.RemoteEndPoint, out _);
                SendClientLeavedChat();
                
                return null;
            }
        }

        private void SendClientLeavedChat()
        {
            //TODO for all users 
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

        private async Task HandleRegisterClientMessage(Socket tcpSocket, RegisterClientMessage registerClientMessage)
        {
            var generatedToken = Guid.NewGuid();
            var clientToken = generatedToken.ToString();
            _clientConnectionToken.TryAdd(tcpSocket.RemoteEndPoint, clientToken);
            var clientRegisteredMessage = new ClientRegisteredMessage
            {
                ClientToken = clientToken
            };
            var packedMessage = MessageConverter.PackMessage(clientRegisteredMessage);
            await tcpSocket.SendAsync(packedMessage, SocketFlags.None);

            var userJoinedChat = new NewUserConnected()
            {
                UserName = registerClientMessage.UserName
            };
            var packedJoinedUserMessage = MessageConverter.PackMessage(userJoinedChat);
            foreach (var connection in _clientsConnections)
            {
                await connection.Value.SendAsync(packedJoinedUserMessage,SocketFlags.None);
            }
        }
    }
}