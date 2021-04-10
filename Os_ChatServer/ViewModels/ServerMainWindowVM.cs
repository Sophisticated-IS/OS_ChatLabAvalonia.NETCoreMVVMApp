using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Os_ChatServer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ConcurrentDictionary<string,Socket> ChatClients { get;}
        
        
        public MainWindowViewModel()
        {
           
        }

        
    }
}