using System.Net.Sockets;
using System.Reactive;
using System.Threading;
using Os_ChatServer.Services;
using ReactiveUI;


namespace Os_ChatServer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private UdpListener _udpListener;
        
        // public ReactiveCommand<Unit, Unit> DoTheThing { get; }

        
        
        public MainWindowViewModel()
        {
            _udpListener = new UdpListener();

            ThreadPool.QueueUserWorkItem(_udpListener.Run);
        }
    }
}