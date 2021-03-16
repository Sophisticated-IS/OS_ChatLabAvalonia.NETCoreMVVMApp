using System.Net;
using System.Net.Sockets;
using Messages.ClientMessage;
using Messages.ServerMessage;
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


                    var decompressedData = ByteCompressor.DeCompress(data);
                    var deserializedMessage = Serializer.DeSerialize(decompressedData);

                    switch (deserializedMessage)
                    {
                        case WhoIsServerMessage:
                            //todo неявная ссылка на константы
                            var answer = new ServerAddressMessage {ServerIP = TcpServer.TcpServerIP,ServerPort = TcpServer.TcpServerPort };
                            var serializedMessage = Serializer.Serialize(answer);
                            var compressedMessage = ByteCompressor.Compress(serializedMessage);
                            _serverUdp.Send(compressedMessage, compressedMessage.Length, senderEndPoint);
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