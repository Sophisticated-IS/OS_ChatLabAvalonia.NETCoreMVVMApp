using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Messages.TftpServerMessages;
using Utils;

namespace Os_ChatServer.Services
{
    public sealed class FtpServer
    {
        private TcpClient _serverTFTP;
        public const int Port = 9999;
        public const string FTPServerIP  = "0.0.0.1";

        public FtpServer()
        {
            _serverTFTP = new TcpClient(FTPServerIP,Port);
        }


        public void Run()
        {
            
        }

        ~FtpServer()
        {
            _serverTFTP?.Close();
            _serverTFTP?.Dispose();
        }
    }
}