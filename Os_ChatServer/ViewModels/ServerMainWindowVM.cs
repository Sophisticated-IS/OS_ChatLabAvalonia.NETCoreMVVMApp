using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reactive;
using System.Threading;
using Os_ChatServer.Models;
using Os_ChatServer.Services;
using ReactiveUI;


namespace Os_ChatServer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private UdpServer _udpServer;
        private TcpServer _tcpServer;
        // private FtpServer _ftpServer;
        public ConcurrentDictionary<string,Socket> ChatClients { get;}
        
        
        public MainWindowViewModel()
        {
            try
            {
                _udpServer = new UdpServer();
                ChatClients = new ConcurrentDictionary<string, Socket>();
                _tcpServer = new TcpServer(ChatClients);
                ThreadPool.QueueUserWorkItem(_udpServer.Run);
                ThreadPool.QueueUserWorkItem(_tcpServer.Run);//WaitCallBack???
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            
        }
    }
}