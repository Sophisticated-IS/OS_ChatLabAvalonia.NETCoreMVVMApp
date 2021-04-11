using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Messages.Base;
using Messages.ClientMessage.AuthorizedClientMessage;
using Messages.ClientMessage.UnauthorizedClientMessage;
using Messages.ServerMessage;
using Utils;

namespace Os_ChatServer.Services
{
    public sealed class RegistrationClientService
    {
        private readonly string _tcpIp;
        private readonly int _tcpPort;
        private ConcurrentDictionary<string, string> _clientTokenName;
        private ConcurrentDictionary<EndPoint, Socket> _clientsConnections;
        private ConcurrentDictionary<string, string> _TokenFilePath;

        public RegistrationClientService(string tcpIp, int tcpPort)
        {
            _clientsConnections = new ConcurrentDictionary<EndPoint, Socket>();
            _clientTokenName = new ConcurrentDictionary<string, string>();
            _TokenFilePath = new ConcurrentDictionary<string, string>();
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
            while (true)
            {
                var clientConnection = await tcpSocket.AcceptAsync();
                _clientsConnections.TryAdd(clientConnection.RemoteEndPoint, clientConnection);
                ThreadPool.QueueUserWorkItem(RunServerMessagesHandler, clientConnection);
            }
        }

        private async void RunServerMessagesHandler(object? state)
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
                            await HandleRegisterClientMessage(clientSocket, registerClientMessage);
                            break;
                        
                        case ClientSendTextMessage clientSendTextMessage:
                            await HandleClientSendTextMessage(clientSocket,clientSendTextMessage);
                            break;
                        
                        case ClientSendFilePartMessage clientSendFilePartMessage:
                            await HandleClientSendFilePartMessage(clientSocket, clientSendFilePartMessage);
                            break;
                    }
                }
            }
        }

        private async Task HandleClientSendTextMessage(Socket clientSocket, ClientSendTextMessage clientSendTextMessage)
        {
            var newTextMessageSent = new NewTextMessageSent
            {
                TextMessage = clientSendTextMessage.TextMessage,
                UserName = _clientTokenName[clientSendTextMessage.ClientToken]
            };
            foreach (var clientConnection in _clientsConnections)
            {
                if (clientConnection.Value.RemoteEndPoint == clientSocket.RemoteEndPoint) continue;
                
                var message = MessageConverter.PackMessage(newTextMessageSent);
                await clientConnection.Value.SendAsync(message,SocketFlags.None);
            }
        }


        private async Task HandleClientSendFilePartMessage(Socket clientSocket, ClientSendFilePartMessage clientSendFilePart)
        {
            await using var fileWriter = new FileStream(clientSendFilePart.FileName,FileMode.Append);
            var data = clientSendFilePart.Data;
            
            if (data.Length == 0)
            {
                var name = _clientTokenName[clientSendFilePart.ClientToken];
                var fileToken = Guid.NewGuid().ToString();
                var fileIsTransferredMessage = new FileWasTransferredMessage
                {
                    FileName = clientSendFilePart.FileName,
                    FileToken = fileToken,
                    UserName = name
                };
                _TokenFilePath.TryAdd(fileToken, clientSendFilePart.FileName);
                var packedFileWasTransferred = MessageConverter.PackMessage(fileIsTransferredMessage);
                await SendDataForEachClient(clientSocket, packedFileWasTransferred);
                await clientSocket.SendAsync(packedFileWasTransferred, SocketFlags.None);
                return;
            }
            
            await fileWriter.WriteAsync(data,0,data.Length);
            var acceptMessage = new ServerSuccessMessage();
            var packedAnswer = MessageConverter.PackMessage(acceptMessage);
            await clientSocket.SendAsync(packedAnswer, SocketFlags.None);
            
        }

        private async Task<int?> TryReceiveData(Socket clientSocket, byte[] buffer, EndPoint clientIpEndPoint)
        {
            try
            {
                var result = await clientSocket.ReceiveFromAsync(buffer, SocketFlags.None, clientIpEndPoint);
                clientIpEndPoint = result.RemoteEndPoint;
                if (result.ReceivedBytes == 0) throw new InvalidDataException();

                return result.ReceivedBytes;
            }
            catch (Exception)
            {
                _clientsConnections.Remove(clientSocket.RemoteEndPoint, out _);
                SendClientLeftChat();
                clientSocket.Close();
                return null;
            }
        }

        private void SendClientLeftChat()
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

            var userJoinedChat = new NewUserConnected
            {
                UserName = registerClientMessage.UserName
            };
            var packedJoinedUserMessage = MessageConverter.PackMessage(userJoinedChat);
            await SendDataForEachClient(tcpSocket, packedJoinedUserMessage);
            
            _clientTokenName.TryAdd(clientToken,registerClientMessage.UserName);
            var users = _clientTokenName.Values.ToArray();
            var clientRegisteredMessage = new ClientRegisteredMessage
            {
                ClientToken = clientToken,
                CurrentUsersListInChat = users
            };
            var packedMessage = MessageConverter.PackMessage(clientRegisteredMessage);
            await tcpSocket.SendAsync(packedMessage, SocketFlags.None);
        }

        private async Task SendDataForEachClient(Socket tcpSocket, byte[] packedJoinedUserMessage)
        {
            foreach (var connection in _clientsConnections)
            {
                if (connection.Value.RemoteEndPoint != tcpSocket.RemoteEndPoint)
                {
                    await connection.Value.SendAsync(packedJoinedUserMessage, SocketFlags.None);
                }
            }
        }
    }
}