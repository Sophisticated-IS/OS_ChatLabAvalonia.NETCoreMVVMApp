using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Messages.Base;
using Messages.ServerMessage;
using Messages.TftpServerMessages;
using Messages.TftpServerMessages.Base;
using Utils;

namespace Os_ChatServer.Services
{
    public sealed class TftpServer
    {
        private Socket _serverTFTP;
        public const int Port = 1025;
        public const string FTPServerIP = "127.0.0.87";

        public TftpServer()
        {
        }


        public async void Run(object? state)
        {
            async Task AcceptFileMessage(Socket clientFile)
            {
                var acceptMessage = new ServerSuccessMessage();
                var packedAccept = MessageConverter.PackMessage(acceptMessage);
                await clientFile.SendAsync(packedAccept, SocketFlags.None);
            }

            EndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(FTPServerIP), Port);
            _serverTFTP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverTFTP.Bind(ipEndPoint);
            _serverTFTP.Listen(1);


            while (true)
            {
                using (var clientFile = await _serverTFTP.AcceptAsync().ConfigureAwait(false))
                {
                    var isFileEndReceiving = false;
                    while (!isFileEndReceiving)
                    {
                        var fullMessage = new List<byte>(1024);
                        do
                        {
                            var buffer = new byte[1024];
                            var res = await clientFile.ReceiveAsync(buffer, SocketFlags.None);
                            fullMessage.AddRange(buffer.Take(res));
                        } while (clientFile.Available > 0);

                        var message = MessageConverter.UnPackMessage<TftpMessage>(fullMessage.ToArray());
                        switch (message)
                        {
                            case StartLoadFileMessage:

                                await AcceptFileMessage(clientFile);
                                break;
                            case SendPartFileMessage:

                                await AcceptFileMessage(clientFile);
                                break;
                            case EndLoadFileMessage:

                                await AcceptFileMessage(clientFile);
                                isFileEndReceiving = true;
                                break;
                        }
                    }
                }
            }
        }

        ~TftpServer()
        {
            _serverTFTP?.Close();
            _serverTFTP?.Dispose();
        }
    }
}