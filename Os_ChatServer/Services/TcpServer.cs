using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;
using Os_ChatServer.Models;

namespace Os_ChatServer.Services
{
    public sealed class TcpServer
    {
        // public ConcurrentDictionary<string, ChatClient> ConcurrentDictionary { get; }
        public const string TcpServerIP= "127.0.0.33";
        public const int TcpServerPort= 8083;

        private Socket _tcpSocket;
        private List<Socket> _tcpConnections;
        public TcpServer([NotNull] ConcurrentDictionary<string, ChatClient> concurrentDictionary)
        {
            // ConcurrentDictionary = concurrentDictionary ?? throw new ArgumentNullException(nameof(concurrentDictionary));
            _tcpConnections = new List<Socket>();
        }

        public void Run(object? _)
        {
            // if (clients == null) throw new ArgumentNullException(nameof(clients));
            
            // var clientsDictionary = (ConcurrentDictionary<string,ChatClient>)clients;
            
            EndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(TcpServerIP), TcpServerPort);
            _tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _tcpSocket.Bind(ipEndPoint);
            _tcpSocket.Listen(10000);
            
            while (true)
            {
                do
                { 
                    var clientSocket = _tcpSocket.Accept();
                    _tcpConnections.Add(clientSocket);//todo не забыть удалять
                } while (_tcpSocket.Available > 0);
            }
        }

        private void StartClientService(Socket tcpSocket)
        {
            //todo + ThreadPool
        }

        ~TcpServer()
        {
            _tcpSocket?.Close();
        }
    }
}