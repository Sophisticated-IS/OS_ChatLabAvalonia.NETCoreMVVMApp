using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
            using (var socket = new TcpClient($"{_serverEndPoint.Address}", _serverEndPoint.Port))
            {
                if (!File.Exists(filePath)) return false;
                //Begin sending file
                var fileName = Path.GetFileName(filePath);
                var startLoadFileMessage = new StartLoadFileMessage {FileName = fileName};
                var startLoadFileMessageBytes = MessageConverter.PackMessage<TftpMessage>(startLoadFileMessage);
                await socket.Client.SendAsync(startLoadFileMessageBytes, SocketFlags.None);

            }

            // do
            // {
            //     var senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
            //     var data = _serverTFTP.Receive(ref senderEndPoint);
            //
            //
            //     var receivedMessage = MessageConverter.UnPackMessage(data);
            //     switch (receivedMessage)
            //     {
            //  
            //     }
            // } while (_serverTFTP.Available > 0);

            return true;
        }

        public async Task<bool> ReceiveFile([NotNull] string fileName)
        {
            throw new NotImplementedException();
        }
    }
}