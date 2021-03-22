using Messages;
using Messages.Base;
using Messages.TftpServerMessages.Base;

namespace Utils
{
    public static class MessageConverter
    {
        public static byte[] PackMessage<T>(T message)  where T : IMessageProvider
        {
            var serializedMessage = Serializer.Serialize(message);
            var compressedMessage = ByteCompressor.Compress(serializedMessage);

            return compressedMessage;
        }

        public static T UnPackMessage<T>(byte[] data)
        {
            var decompressData = ByteCompressor.DeCompress(data);
            var deserializedMessage = Serializer.DeSerialize<T>(decompressData);
            
            return deserializedMessage;
        }
    }
}