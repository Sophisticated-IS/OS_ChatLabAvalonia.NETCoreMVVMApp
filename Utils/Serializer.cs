using System;
using System.IO;
using Messages.Base;
using Protobuf = ProtoBuf.Serializer;

namespace Utils
{
    public static class Serializer
    {
        public static byte[] Serialize(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            using (var memStr = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(memStr, message);
                return memStr.ToArray();
            }
        }

        public static Message DeSerialize(byte[] bytes) 
        {
            using (var memStr = new MemoryStream(bytes))
            {
                return ProtoBuf.Serializer.Deserialize<Message>(memStr);
            }
        }
    }
}