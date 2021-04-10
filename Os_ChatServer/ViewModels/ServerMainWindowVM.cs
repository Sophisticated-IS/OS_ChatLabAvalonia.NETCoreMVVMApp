using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Os_ChatServer.Services;

namespace Os_ChatServer.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public const string TcpIp = "127.0.0.133";
        public const int TcpPort = 12301;
        
        public ConcurrentDictionary<string,Socket> ChatClients { get;}
        private ClientFinderService _clientFinderService;
        private RegistrationClientService _registrationClientService;
        
        
        public MainWindowViewModel()
        {
            _clientFinderService = new ClientFinderService(TcpIp,TcpPort);
            _registrationClientService = new RegistrationClientService(TcpIp, TcpPort);
        }

        
    }
}