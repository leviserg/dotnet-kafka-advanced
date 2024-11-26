using Confluent.Kafka;
using System.Text.Json;

namespace message_contract
{
    public class JsonToByteArraySerializer<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            return JsonSerializer.SerializeToUtf8Bytes(data);
        }
    }
}
