using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Messages.ClientMessage;
using Messages.ServerMessage;
using Utils;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Services
{
    public sealed class UdpClientSender
    {
        public const int ServerPort = 8081;
        public const int ClientPort = 8082;
        
        private UdpClient _udpClient;
        
        public UdpClientSender()
        {
            
        }
        
        public async Task<IPEndPoint?> FindServer()
        {
            try
            {
                _udpClient = new UdpClient(ClientPort);
          
                var whoIsServerMessage = new WhoIsServerMessage();
                var serializedMessage = Serializer.Serialize(whoIsServerMessage);
                var compressedMessage = ByteCompressor.Compress(serializedMessage); 
            
                while (true)
                {
                    await _udpClient.SendAsync(compressedMessage, compressedMessage.Length, "255.255.255.255", ServerPort);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    if (_udpClient.Available > 0)
                    {
                        var serverEndPoint = new IPEndPoint(IPAddress.Any,0);
                        var answer = _udpClient.Receive(ref serverEndPoint);
                        var decompressedData = ByteCompressor.DeCompress(answer);
                        var deserializedMessage = Serializer.DeSerialize(decompressedData);

                        switch (deserializedMessage)
                        {
                            case ServerAddressMessage serverAddressMessage:
                                return new IPEndPoint(IPAddress.Parse(serverAddressMessage.ServerIP),serverAddressMessage.ServerPort);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                return null;
            }
        }
        
        ~UdpClientSender()
        {
            _udpClient?.Close();
        }
    }
}