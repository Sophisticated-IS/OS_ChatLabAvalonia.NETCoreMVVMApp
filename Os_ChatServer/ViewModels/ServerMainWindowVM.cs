using System.Collections.Concurrent;
using System.Net.Sockets;
using Os_ChatServer.Services;

namespace Os_ChatServer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ConcurrentDictionary<string,Socket> ChatClients { get;}
        private ClientFinderService _clientFinderService;
        
        public MainWindowViewModel()
        {
            _clientFinderService = new ClientFinderService();
        }

        
    }
}