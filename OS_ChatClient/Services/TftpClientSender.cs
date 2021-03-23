using System;
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

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Services
{
    public class TftpClientSender
    {
        private readonly IPEndPoint _serverEndPoint;

        public TftpClientSender(IPEndPoint serverEndPoint)
        {
            _serverEndPoint = serverEndPoint;
        }

        public async Task<bool> SendFile(string filePath)
        {
            async Task WaitForResponse(TcpClient socket)
            {
                var ACKBuffer = new byte [128];
                var bytesAmount = await socket.Client.ReceiveAsync(ACKBuffer, SocketFlags.None);
                var data = ACKBuffer.Take(bytesAmount).ToArray();
                var receivedMessage = MessageConverter.UnPackMessage<Message>(data);

                if (receivedMessage is ServerSuccessMessage)
                {
                    //todo
                }
            }

            async Task SendFilePartBytes(byte[] sendBuffer, TcpClient socket)
            {
                var sendFilePartMessage = new SendPartFileMessage {BytesData = sendBuffer};
                var packedMessage = MessageConverter.PackMessage(sendFilePartMessage);

                await socket.Client.SendAsync(packedMessage, SocketFlags.None);

                await WaitForResponse(socket);
            }

            using (var socket = new TcpClient($"{_serverEndPoint.Address}", _serverEndPoint.Port))
            {
                if (!File.Exists(filePath)) return false;
                //Begin sending file
                var fileName = Path.GetFileName(filePath);
                var startLoadFileMessage = new StartLoadFileMessage {FileName = fileName};
                var startLoadFileMessageBytes = MessageConverter.PackMessage<TftpMessage>(startLoadFileMessage);
                await socket.Client.SendAsync(startLoadFileMessageBytes, SocketFlags.None);
                await WaitForResponse(socket);

                var fileBytes = await File.ReadAllBytesAsync(filePath);
                await SendFilePartBytes(fileBytes, socket);
                
                var fileStopLoadMessage = new EndLoadFileMessage();
                var packedEndFileMessage = MessageConverter.PackMessage(fileStopLoadMessage);
                await socket.Client.SendAsync(packedEndFileMessage, SocketFlags.None);
                await WaitForResponse(socket);
            }

            return true;
        }

        public async Task<bool> ReceiveFile([NotNull] string fileName)
        {
            throw new NotImplementedException();
        }
    }
}