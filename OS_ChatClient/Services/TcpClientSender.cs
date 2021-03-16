﻿

using System;
using System.Net;
using System.Net.Sockets;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Services
{
    public sealed class TcpClientSender
    {
        public const string TcpClientIP= "127.0.0.33";
        public const int TcpClientPort= 8084;

        private Socket _tcpSocket;
        
        public TcpClientSender(IPEndPoint serverIpEndPoint)
        {
            SetConnection(serverIpEndPoint);
        }


        private bool SetConnection(IPEndPoint serverIpEndPoint)
        {
            try
            {
                var ipEndPoint = new IPEndPoint(IPAddress.Parse(TcpClientIP), TcpClientPort);

               _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
               _tcpSocket.Bind(ipEndPoint);
               
               _tcpSocket.Connect(serverIpEndPoint);
               
               return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        
        public void Run()
        {
            
        }

        ~TcpClientSender()
        {
            _tcpSocket?.Disconnect(false);
            _tcpSocket?.Close();
        }
    }
}