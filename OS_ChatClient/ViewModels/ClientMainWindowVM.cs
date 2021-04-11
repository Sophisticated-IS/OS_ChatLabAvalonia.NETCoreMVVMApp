using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Reactive;
using Messages.ClientMessage.AuthorizedClientMessage;
using Messages.ServerMessage;
using OS_ChatLabAvalonia.NETCoreMVVMApp.Models;
using OS_ChatLabAvalonia.NETCoreMVVMApp.Services;
using ReactiveUI;

namespace OS_ChatLabAvalonia.NETCoreMVVMApp.ViewModels
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private readonly string _userName;
        private readonly string _userToken;
        private readonly IPEndPoint _serverEndPoint;
        private readonly ServerMessagesListener _serverMessagesListener;
        public ReactiveCommand<Unit, Unit> SendTextMessageCommand { get; }
        public string ChatMessageText { get; set; }

        public ObservableCollection<ChatMessage> ChatMessages { get; set; }
        public ObservableCollection<string> UsersInChat { get; set; }
        public MainWindowViewModel(string userName,string userToken,string[] usersInChat,IPEndPoint serverEndPoint)
        {
            _userName = userName;
            _userToken = userToken;
            _serverEndPoint = serverEndPoint;
            ChatMessages = new ObservableCollection<ChatMessage>();
            for (int i = 0; i < usersInChat.Length; i++)
            {
                if (usersInChat[i] == userName)
                {
                    usersInChat[i] += "(YOU)";
                }
            }
            UsersInChat = new ObservableCollection<string>(usersInChat);
            _serverMessagesListener = new ServerMessagesListener(serverEndPoint);
            SendTextMessageCommand = ReactiveCommand.Create(SendTextMessage);
            
            _serverMessagesListener.NewUserConnectedEvent += ServerMessagesListenerOnNewUserConnectedEvent;
            _serverMessagesListener.TextMessageSentEvent += ServerMessagesListenerOnTextMessageSentEvent;
        }

        private void ServerMessagesListenerOnTextMessageSentEvent(NewTextMessageSent newTextMessageSent)
        {
            ChatMessages.Add(new ChatMessage(newTextMessageSent.UserName,newTextMessageSent.TextMessage,DateTime.Now, false));
        }

        private void ServerMessagesListenerOnNewUserConnectedEvent(NewUserConnected newUserConnected)
        {
            UsersInChat.Add(newUserConnected.UserName);
        }

        private async void SendTextMessage()
        {
            var textMessage = new ClientSendTextMessage
            {
                ClientToken = _userToken,
                TextMessage = ChatMessageText
            };

            var result =await _serverMessagesListener.SendMessage(textMessage);
            if (result)
            {
                ChatMessages.Add(new ChatMessage(_userName,ChatMessageText,DateTime.Now,false));
            }
        }
    }
}