using System.Net;
using System.Net.Sockets;
using Messages.ClientMessage;
using Messages.ServerMessage;
using Utils;

namespace Os_ChatServer.Services
{
    public class UdpListener
    {
        public const int ServerPort = 8081;
        public const string ServerIP= "127.0.0.1";
        public UdpListener()
        {
        }

        public void Run(object? state)
        {
            var serverUdp = new UdpClient(ServerPort);

            while (true)
            {
                
                while (serverUdp.Available > 0)
                {
                    var senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    var data = serverUdp.Receive(ref senderEndPoint);
                    
                    
                    var decompressedData = ByteCompressor.DeCompress(data);
                    var deserializedMessage = Serializer.DeSerialize(decompressedData);

                    switch (deserializedMessage)
                    {
                        case WhoIsServerMessage :
                            var answer = new ServerAddressMessage{ServerIP = ServerIP};
                            var serializedMessage = Serializer.Serialize(answer);
                            var compressedMessage = ByteCompressor.Compress(serializedMessage);
                            serverUdp.Send(compressedMessage,compressedMessage.Length,senderEndPoint);
                            break;
                    }
                }
                
            }
        }
        
    }
}