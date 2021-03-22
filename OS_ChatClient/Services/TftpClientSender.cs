using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Messages.TftpServerMessages;
using Utils;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Services
{
    public class TftpClientSender
    {
        private TcpClient _serverTFTP;
        public const int Port = 12300;
        public const string TftpClientIP = "0.0.0.3";
        public TftpClientSender(IPEndPoint serverEndPoint)
        {
            _serverTFTP = new TcpClient(TftpClientIP,Port); 
            _serverTFTP.Connect(serverEndPoint);
        }
        
        public async Task<bool> SendFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;
            //Begin sending file
            var fileName = Path.GetFileName(filePath);
            var startLoadFileMessage = new StartLoadFileMessage{FileName = fileName};
            var startLoadFileMessageBytes = MessageConverter.PackMessage(startLoadFileMessage);
            await _serverTFTP.Client.SendAsync(startLoadFileMessageBytes,SocketFlags.None);

            
          
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


        ~TftpClientSender()
        {
            _serverTFTP?.Close();
            _serverTFTP?.Dispose();
        }
    }
}