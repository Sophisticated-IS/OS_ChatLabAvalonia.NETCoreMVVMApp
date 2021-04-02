using Messages.Base;

namespace Utils
{
    public static class MessageConverter
    {
        public static byte[] PackMessage<T>(Message message)
        {
            var serializedMessage = Serializer.Serialize(message);
            var compressedMessage = ByteCompressor.Compress(serializedMessage);

            return compressedMessage;
        }

        public static Message UnPackMessage(byte[] data)
        {
            var decompressData = ByteCompressor.DeCompress(data);
            var deserializedMessage = Serializer.DeSerialize(decompressData);
            
            return deserializedMessage;
        }
    }
}