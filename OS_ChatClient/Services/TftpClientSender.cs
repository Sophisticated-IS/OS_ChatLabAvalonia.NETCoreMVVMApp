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
            async Task SendFilePartBytes(byte[] sendBuffer, TcpClient socket)
            {
                var sendFilePartMessage = new SendPartFileMessage {BytesData = sendBuffer};
                var packedMessage = MessageConverter.PackMessage(sendFilePartMessage);

                await socket.Client.SendAsync(packedMessage, SocketFlags.None);

                var ACKBuffer = new byte [128];
                var bytesAmount = await socket.Client.ReceiveAsync(ACKBuffer, SocketFlags.None);
                var data = ACKBuffer.Take(bytesAmount).ToArray();
                var receivedMessage = MessageConverter.UnPackMessage<Message>(data);

                if (receivedMessage is ServerSuccessMessage)
                {
                    //todo
                }
            }

            using (var socket = new TcpClient($"{_serverEndPoint.Address}", _serverEndPoint.Port))
            {
                if (!File.Exists(filePath)) return false;
                //Begin sending file
                var fileName = Path.GetFileName(filePath);
                var startLoadFileMessage = new StartLoadFileMessage {FileName = fileName};
                var startLoadFileMessageBytes = MessageConverter.PackMessage<TftpMessage>(startLoadFileMessage);
                await socket.Client.SendAsync(startLoadFileMessageBytes, SocketFlags.None);

                var fileBytes = await File.ReadAllBytesAsync(filePath);
                var sendBuffer = new List<byte>(2048);
                for (int i = 0; i < fileBytes.Length; i++)
                {
                    if (sendBuffer.Capacity == sendBuffer.Count)
                    {
                        await SendFilePartBytes(sendBuffer.ToArray(), socket);
                        sendBuffer.Clear();
                    }
                    
                    sendBuffer.Add(fileBytes[i]);
                }

                if (sendBuffer.Count!=0)
                {
                    var sendOthersBytes = sendBuffer.Take(sendBuffer.Count).ToArray();
                    await SendFilePartBytes(sendOthersBytes, socket);
                }

                var fileStopLoadMessage = new EndLoadFileMessage();
                var packedEndFileMessage = MessageConverter.PackMessage(fileStopLoadMessage);
                await socket.Client.SendAsync(packedEndFileMessage, SocketFlags.None);
                
            }

            return true;
        }

        public async Task<bool> ReceiveFile([NotNull] string fileName)
        {
            throw new NotImplementedException();
        }
    }
}