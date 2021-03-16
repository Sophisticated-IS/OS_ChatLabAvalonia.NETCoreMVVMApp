using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reactive;
using System.Text;
using System.Threading;
using Messages.Base;
using Messages.ClientMessage;
using OS_ChatLabAvalonia.NETCoreMVVMApp.Services;
using ReactiveUI;
using Utils;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // public ReactiveCommand<Unit, Unit> DoTheThing { get; }

        private string _serverIP;
        public MainWindowViewModel()
        {
            RunTheThing();
            // DoTheThing = ReactiveCommand.Create(RunTheThing);
        }

        private async void RunTheThing()
        {
            _serverIP = await new UdpServerFinder().FindServer();
        }
    }
}