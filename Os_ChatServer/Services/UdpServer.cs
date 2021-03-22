using System.Net;
using System.Net.Sockets;
using Messages.Base;
using Messages.ClientMessage.NotAuthorizedUserMessages;
using Messages.ServerMessage.ServerAddressMessage;
using Utils;

namespace Os_ChatServer.Services
{
    public sealed class UdpServer
    {
        public const int UdpServerPort = 8081;

        private UdpClient _serverUdp;

        public UdpServer()
        {
        }

        public void Run(object? state)
        {
            _serverUdp = new UdpClient(UdpServerPort);

            while (true)
            {

                do
                {
                    var senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    var data = _serverUdp.Receive(ref senderEndPoint);


                    
                    var receivedMessage = MessageConverter.UnPackMessage<Message>(data);
                    switch (receivedMessage)
                    {
                        case WhoIsServerMessage:
                            //todo неявная ссылка на константы
                            var answer = new TextServerAddressMessage
                            {
                                ServerIP = TcpServer.TcpServerIP,
                                ServerPort = TcpServer.TcpServerPort
                            };
                            
                            var sendingMessage = MessageConverter.PackMessage(answer);
                            _serverUdp.Send(sendingMessage, sendingMessage.Length, senderEndPoint);
                            break;
                    }
                    
                } while (_serverUdp.Available > 0);

            }
        }
        
        ~UdpServer()
        {
            _serverUdp?.Close();
        }
    }
}