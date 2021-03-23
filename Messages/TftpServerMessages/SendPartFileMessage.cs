﻿using Messages.TftpServerMessages.Base;
using ProtoBuf;

namespace Messages.TftpServerMessages
{
    [ProtoContract]
    public sealed class SendPartFileMessage : TftpMessage
    {
        [ProtoMember(1)]
        public  byte[] BytesData { get; set; }
    }
}