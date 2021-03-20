using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Messages.Base;
using Messages.ClientMessage.NotAuthorizedUserMessages;
using Messages.ServerMessage;
using Utils;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Services
{
    public sealed class TcpClientSender
    {
        public string TcpClientIP;
        public int TcpClientPort;

        private Socket _tcpSocket;

        private static List<string> TcpIPs = new List<string>(){"127.0.0.98","127.0.0.99"}; 
        private static List<int> TcpPorts = new List<int>(){8084,8094};

        public TcpClientSender(IPEndPoint serverIpEndPoint)
        {

            var Mutex = new Mutex(true, "EB617FE4-1610-4FE9-82B7-853EC0D77F24");

            if (Mutex.WaitOne(TimeSpan.Zero))
            {
                TcpClientIP = TcpIPs.First();
                TcpClientPort = TcpPorts.First();
            }
            else
            {
                TcpClientIP = TcpIPs.Last();
                TcpClientPort = TcpPorts.Last();
            }
            
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
            catch (Exception)
            {
                return false;
            }
        }

        public async void SendMessage(Message message)
        {
            var sendingMessage = MessageConverter.PackMessage(message);
            await _tcpSocket.SendAsync(sendingMessage, SocketFlags.None); //TODO доходит
            //TODO добавить регистрацию
            //TODO 
            //TODO
        }

        public async Task<bool> RegisterUser(string name)
        {
            if (string.IsNullOrEmpty(name)) return false;
            var registerNewUserMessage = new RegisterNewUserMessage {NewUserName = name};
            var sendingMessage = MessageConverter.PackMessage(registerNewUserMessage);
            await _tcpSocket.SendAsync(sendingMessage, SocketFlags.None);

            var data = new byte[1024];
            var bytesAmount = await _tcpSocket.ReceiveAsync(data, SocketFlags.None);

            var answerMessage = data.Take(bytesAmount).ToArray();
            var resultMessage = MessageConverter.UnPackMessage(answerMessage);

            switch (resultMessage)
            {
                case ServerFailMessage serverFailMessage:
                    return false;

                case ServerSuccessMessage serverSuccessMessage:
                    return true;

                default: return false;
            }
        }

        ~TcpClientSender()
        {
            _tcpSocket?.Disconnect(false);
            _tcpSocket?.Close();
        }
    }
}