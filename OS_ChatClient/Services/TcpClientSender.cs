using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Messages.Base;
using Messages.ClientMessage.NotAuthorizedUserMessages;
using Messages.ServerMessage;
using Utils;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.Services
{
    public sealed class TcpClientSender
    {
        public const string TcpClientIP = "127.0.0.99";
        public const int TcpClientPort = 8084;

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