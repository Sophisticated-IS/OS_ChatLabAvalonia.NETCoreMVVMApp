using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Messages.ClientMessage;
using Messages.ServerMessage;
using Utils;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Services
{
    public class UdpServerFinder
    {
        public const int ServerPort = 8081;
        public const int ClientPort = 8082;
        public const string ClientIP = "127.0.0.1";
        
        public UdpServerFinder()
        {
            
        }
        
        public async Task<string> FindServer()
        {
            var udpClient = new UdpClient();
            var clientEndPoint = new IPEndPoint(IPAddress.Parse(ClientIP), ClientPort);
            udpClient.Client.Bind(clientEndPoint);

            var whoIsServerMessage = new WhoIsServerMessage();
            var serializedMessage = Serializer.Serialize(whoIsServerMessage);
            var compressedMessage = ByteCompressor.Compress(serializedMessage); 
            
            while (true)
            {
                await udpClient.SendAsync(compressedMessage, compressedMessage.Length, "255.255.255.255", ServerPort);
                await Task.Delay(TimeSpan.FromSeconds(1));
                if (udpClient.Available > 0)
                {
                    var serverEndPoint = new IPEndPoint(IPAddress.Any,0);
                    var answer = udpClient.Receive(ref serverEndPoint);
                    var decompressedData = ByteCompressor.DeCompress(answer);
                    var deserializedMessage = Serializer.DeSerialize(decompressedData);

                    switch (deserializedMessage)
                    {
                        case ServerAddressMessage serverAddressMessage:
                            return serverAddressMessage.ServerIP;
                    }
                }
            }
        }
    }
}