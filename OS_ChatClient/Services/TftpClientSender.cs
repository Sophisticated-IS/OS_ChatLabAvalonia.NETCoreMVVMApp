using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
                var sendFilePartMessage = new ReceivePartFileMessage {BytesData = sendBuffer};
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

        public async Task<bool> ReceiveFile([NotNull] string fileName,string fileFullPath)
        {
            using (var socket = new TcpClient($"{_serverEndPoint.Address}", _serverEndPoint.Port))
            {
                var askForfileexistance = new AskForStartSendingFileMessage() {FileName = fileName};
                var packedAskForFileStartReceiving = MessageConverter.PackMessage(askForfileexistance);
                await socket.Client.SendAsync(packedAskForFileStartReceiving, SocketFlags.None);

                var bufferACK = new byte[128];
                var realBytes = await socket.Client.ReceiveAsync(bufferACK, SocketFlags.None);
                var answerMessage = MessageConverter.UnPackMessage<Message>(bufferACK.Take(realBytes).ToArray());
                if (!(answerMessage is ServerSuccessMessage)) return false;

                var getFilePartMessage = new AskForSendFilePartMessage();
                var packedAskFilePart = MessageConverter.PackMessage(getFilePartMessage);
                await socket.Client.SendAsync(packedAskFilePart, SocketFlags.None);

                var fileBytes = new List<byte>();
                do
                {
                    var buffer = new byte[2048];
                    var bytesAmount = await socket.Client.ReceiveAsync(buffer, SocketFlags.None);
                    fileBytes.AddRange(buffer.Take(bytesAmount));
                } while (socket.Client.Available > 0);

                var filesData = MessageConverter.UnPackMessage<ReceivePartFileMessage>(fileBytes.ToArray());

                if (!File.Exists(fileFullPath))
                {
                    await File.WriteAllBytesAsync(fileFullPath,filesData.BytesData,CancellationToken.None);
                }
                
            }

            return true;
        }
    }
}