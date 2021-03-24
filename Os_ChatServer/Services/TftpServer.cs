using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Messages.ServerMessage;
using Messages.TftpServerMessages;
using Messages.TftpServerMessages.Base;
using Utils;

namespace Os_ChatServer.Services
{
    public sealed class TftpServer
    {
        private Socket _serverTFTP;
        public const int Port = 1025;
        public const string FTPServerIP = "127.0.0.87";

        public TftpServer()
        {
        }


        public async void Run(object? state)
        {
            async Task SendAcceptFileMessage(Socket clientFile)
            {
                var acceptMessage = new ServerSuccessMessage();
                var packedAccept = MessageConverter.PackMessage(acceptMessage);
                await clientFile.SendAsync(packedAccept, SocketFlags.None);
            }

            EndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(FTPServerIP), Port);
            _serverTFTP = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverTFTP.Bind(ipEndPoint);
            _serverTFTP.Listen(1);


            while (true)
            {
                using (var clientFile = await _serverTFTP.AcceptAsync().ConfigureAwait(false))
                {
                    var resultFile = new List<byte>(1024);
                    string filePath = null;
                    string fileSendToClientPAth = null;
                    var isFileEndReceiving = false;
                    while (!isFileEndReceiving)
                    {
                        var fullMessage = new List<byte>(1024);
                        do
                        {
                            var buffer = new byte[1024];
                            var res = await clientFile.ReceiveAsync(buffer, SocketFlags.None);
                            fullMessage.AddRange(buffer.Take(res));
                        } while (clientFile.Available > 0);

                        var message = MessageConverter.UnPackMessage<TftpMessage>(fullMessage.ToArray());
                        switch (message)
                        {
                            case StartLoadFileMessage startLoadFileMessage:

                                const string? dir = "files";
                                // var basePath = AppDomain.CurrentDomain.BaseDirectory;
                                if (!Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }
                                filePath = $"{AppDomain.CurrentDomain.BaseDirectory}{dir}/{startLoadFileMessage.FileName}";
                       
                                
                                await SendAcceptFileMessage(clientFile);
                                break;
                            case ReceivePartFileMessage sendPartFileMessage:
                                resultFile.AddRange(sendPartFileMessage.BytesData);
                                await SendAcceptFileMessage(clientFile);
                                break;
                            case EndLoadFileMessage:

                                
                                using (var stream = File.Open(filePath, FileMode.OpenOrCreate))
                                {

                                    var buff = resultFile.ToArray();
                                    await stream.WriteAsync(buff, 0, buff.Length);
                                }
                                await SendAcceptFileMessage(clientFile);
                                isFileEndReceiving = true;
                                break;
                            
                            case AskForStartSendingFileMessage startSendingFileMessage:
                                var fileONSERVErPATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                    $"files/{startSendingFileMessage.FileName}");
                                if (File.Exists(fileONSERVErPATH))
                                {
                                    await SendAcceptFileMessage(clientFile);
                                    fileSendToClientPAth = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                        $"files/{startSendingFileMessage.FileName}");
                                }
                                break;
                            case AskForSendFilePartMessage sendFilePartMessage:
                                using (var stream = File.Open(fileSendToClientPAth, FileMode.Open))
                                {
                                    var allBytesFromFile = new List<byte>(1024);
                                    int byt;
                                    while ((byt = stream.ReadByte())!=-1)
                                    {
                                        var byteCasted = (byte) byt;
                                        allBytesFromFile.Add(byteCasted);
                                    }
                                    var sendingFilePart = new ReceivePartFileMessage {BytesData = allBytesFromFile.ToArray()};
                                    var sendingData = MessageConverter.PackMessage(sendingFilePart);
                                    await clientFile.SendAsync(sendingData, SocketFlags.None);
                                    isFileEndReceiving = true;
                                }

                              
                                break;
                            
                            
                        }
                    }
                }
            }
        }

        ~TftpServer()
        {
            _serverTFTP?.Close();
            _serverTFTP?.Dispose();
        }
    }
}