using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Utils;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Services
{
    public class ServerMessagesListener
    {
        public const string BaseIpAddress = "127.0.0.";
        private int _port;
        private string _ipAddress;
        
        public ServerMessagesListener()
        {
            _port = PortProvider.GetAvailablePort();
            _ipAddress = GetAvailableIp();
            ThreadPool.QueueUserWorkItem(StartListenForServerMessages);
        }

        private string GetAvailableIp()
        {
            string fullAddress = string.Empty;
            for (int i = 5; i < 255; i++)
            {
                fullAddress = BaseIpAddress + i;
                try
                {
                    var tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    EndPoint endPoint = new IPEndPoint(IPAddress.Parse(fullAddress), _port);
                    tcpClient.Bind(endPoint);
                    return fullAddress;
                }
                catch (Exception e)
                {
                    continue;
                }
            }

            throw new ArgumentOutOfRangeException();
        }
        
        private void StartListenForServerMessages(object? state)
        {
            
        }
    }
}