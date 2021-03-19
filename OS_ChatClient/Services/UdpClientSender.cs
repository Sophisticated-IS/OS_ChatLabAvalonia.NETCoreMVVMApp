using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Messages.ClientMessage.NotAuthorizedUserMessages;
using Messages.ServerMessage.ServerAddressMessage;
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
                var sendingMessage = MessageConverter.PackMessage(whoIsServerMessage); 
            
                while (true)
                {
                    await _udpClient.SendAsync(sendingMessage, sendingMessage.Length, "255.255.255.255", ServerPort);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    if (_udpClient.Available > 0)
                    {
                        var serverEndPoint = new IPEndPoint(IPAddress.Any,0);
                        var answer = _udpClient.Receive(ref serverEndPoint);
                        
                        var receivedMessage = MessageConverter.UnPackMessage(answer);
                        switch (receivedMessage)
                        {
                            case TextServerAddressMessage serverAddressMessage:
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