using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Messages.ClientMessage.AuthorizedUserMessages;
using Messages.ClientMessage.NotAuthorizedUserMessages;
using Messages.ServerMessage;
using Messages.ServerMessage.ServerAddressMessage;
using Utils;

namespace Os_ChatServer.Services
{
    public sealed class TcpServer
    {
        public ConcurrentDictionary<string, Socket> ClientsConcurrentDictionary { get; }
        public const string TcpServerIP = "127.0.0.33";
        public const int TcpServerPort = 8083;

        private Socket _tcpSocket;
        private List<Socket> _tcpConnections;

        public TcpServer([NotNull] ConcurrentDictionary<string, Socket> clientsConcurrentDictionary)
        {
            ClientsConcurrentDictionary = clientsConcurrentDictionary ??
                                          throw new ArgumentNullException(nameof(clientsConcurrentDictionary));
            _tcpConnections = new List<Socket>();
        }

        public async void Run(object? _)
        {
            EndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(TcpServerIP), TcpServerPort);
            _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _tcpSocket.Bind(ipEndPoint);
            _tcpSocket.Listen(10000);

            while (true)
            {
                do
                {
                    var clientSocket = await _tcpSocket.AcceptAsync().ConfigureAwait(false);

                    #region serviceClient

                    _tcpConnections.Add(clientSocket); //todo не забыть удалять
                    ThreadPool.QueueUserWorkItem(StartClientService, clientSocket);

                    #endregion
                } while (_tcpSocket.Available > 0);
            }
        }

        private async void StartClientService(object? tcpSocket)
        {
            Task<int> ReceiveAsync(Socket receivingSocket, byte[] buffer, int offset, int size, SocketFlags flags)
            {
                return Task<int>.Factory.FromAsync(
                    receivingSocket.BeginReceive(buffer, offset, size, flags, null, null), receivingSocket.EndReceive);
            }

            if (tcpSocket is null) return;

            var socket = (Socket) tcpSocket;

            var clientConnected = true;
            while (clientConnected)
            {
                var fullMessage = new List<byte>(1024);
                do
                {
                    var data = new byte[1024];
                    try
                    {
                        var bytesAmount = await ReceiveAsync(socket, data, 0, data.Length, SocketFlags.None)
                            .ConfigureAwait(false);
                        var readBytes =
                            data.Take(bytesAmount); //TODO OPtimize through if the buffer.length equals to bytesAmount
                        fullMessage.AddRange(readBytes);
                    }
                    catch (SocketException socketException)
                    {
                        clientConnected = false;
                    }
                } while (socket.Available > 0);

                if (!clientConnected) return;

                var receivedMessage = MessageConverter.UnPackMessage(fullMessage.ToArray());
                switch (receivedMessage)
                {
                    case RegisterNewUserMessage registerNewUserMessage:
                        var registerUserName = registerNewUserMessage.NewUserName;
                        var isUserRegistered = ClientsConcurrentDictionary.TryAdd(registerUserName, socket);
                        if (isUserRegistered)
                        {
                            var successMessage = new ServerSuccessMessage();
                            var sendingMessage = MessageConverter.PackMessage(successMessage);
                            await socket.SendAsync(sendingMessage, SocketFlags.None, CancellationToken.None)
                                .ConfigureAwait(false);
                        }
                        else
                        {
                            var failMessage = new ServerFailMessage();
                            var sendingMessage = MessageConverter.PackMessage(failMessage);
                            await socket.SendAsync(sendingMessage, SocketFlags.None, CancellationToken.None)
                                .ConfigureAwait(false);
                        }

                        break;

                    case SendTextMessage sendTextMessage:
                        foreach (var client in ClientsConcurrentDictionary.Values)
                        {
                            //посылаем сообщение ко всем клиентам, кроме того от которого пришло это сообщение
                            if (socket.RemoteEndPoint != client.RemoteEndPoint)
                            {
                                var sendingMessage = MessageConverter.PackMessage(sendTextMessage);
                                await client.SendAsync(sendingMessage, SocketFlags.None).ConfigureAwait(false);
                            }
                        }

                        break;

                    case GetFilesServerAddressMessage:
                    {
                        var filesServerAddressMessage = new FilesServerAddressMessage
                        {
                            ServerPort = FtpServer.Port,
                            ServerIP = FtpServer.FTPServerIP
                        };
                        var sendingMessage = MessageConverter.PackMessage(filesServerAddressMessage);
                        await socket.SendAsync(sendingMessage, SocketFlags.None, CancellationToken.None)
                            .ConfigureAwait(false);
                    }
                        break;
                }
            }
        }

        ~TcpServer()
        {
            _tcpSocket?.Close();
        }
    }
}