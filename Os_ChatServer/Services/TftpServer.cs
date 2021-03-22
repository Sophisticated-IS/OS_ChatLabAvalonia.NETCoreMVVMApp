using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Messages.Base;
using Messages.TftpServerMessages;
using Messages.TftpServerMessages.Base;
using Utils;

namespace Os_ChatServer.Services
{
    public sealed class TftpServer
    {
        private Socket _serverTFTP;
        public const int Port = 9999;
        public const string FTPServerIP = "192.168.168.168";

        public TftpServer()
        {
        }


        public async void Run(object? state)
        {
            EndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(FTPServerIP), Port);
            _serverTFTP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverTFTP.Bind(ipEndPoint);
            _serverTFTP.Listen(1);

            
            while (true)
            {
                var fullMessage = new List<byte>(1024);
                var clientFile = await _serverTFTP.AcceptAsync().ConfigureAwait(false);
                
                do
                {
                    var buffer = new byte[1024];
                    var res = await clientFile.ReceiveAsync(buffer,SocketFlags.None);
                    fullMessage.AddRange(buffer.Take(res));
                } while (clientFile.Available > 0);

                var message = MessageConverter.UnPackMessage<TftpMessage>(fullMessage.ToArray());
                
                switch (message)
                {
                    
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