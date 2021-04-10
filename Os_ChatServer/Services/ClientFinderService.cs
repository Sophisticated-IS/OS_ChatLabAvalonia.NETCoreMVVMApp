using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Messages.Base;
using Messages.ClientMessage.UnauthorizedClientMessage;
using Messages.ServerMessage;
using Utils;

namespace Os_ChatServer.Services
{
    public sealed class ClientFinderService
    {
        public const int Port = 12300;
        public const string BroadcastIpAddress = "127.0.0.55"; //"10.255.255.255";
        private readonly string _tcpIp;
        private readonly int _tcpPort;
        
        public ClientFinderService(string tcpIp, int tcpPort)
        {
            _tcpIp = tcpIp;
            _tcpPort = tcpPort;
            ThreadPool.QueueUserWorkItem(RunUdpClientService);
        }
        private async void RunUdpClientService(object? state)
        {
            var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
            {
                EnableBroadcast = true
            };
            EndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(BroadcastIpAddress), Port);
            udpSocket.Bind(ipEndPoint);

            while (true)
            {
                var fullData = new List<byte>(1024);
                EndPoint clientIpEndPoint = new IPEndPoint(IPAddress.None, 0);
                do
                {
                    var buffer = new byte[128];
                    var receiveFromAsync = await udpSocket.ReceiveFromAsync(buffer, SocketFlags.None, clientIpEndPoint);
                    var receivedBytes = buffer.Take(receiveFromAsync.ReceivedBytes);
                    clientIpEndPoint = receiveFromAsync.RemoteEndPoint;
                    fullData.AddRange(receivedBytes);
                } while (udpSocket.Available > 0);

                var receivedMessage = TryUnpackMessage(fullData.ToArray());
                if (receivedMessage != null)
                {
                    switch (receivedMessage)
                    {
                        
                        case WhoIsServerMessage _:
                            var serverAddressMessage = new ServerAddressMessage
                            {
                                Ip = _tcpIp,
                                Port = (uint)_tcpPort
                            };
                            var packedMessage = MessageConverter.PackMessage(serverAddressMessage);
                            await udpSocket.SendToAsync(packedMessage, SocketFlags.None, clientIpEndPoint);
                            break;
                    }
                }
            }
        }

        private Message? TryUnpackMessage(in byte[] data)
        {
            try
            {
                var unpackedMessage = MessageConverter.UnPackMessage(data);
                return unpackedMessage;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}