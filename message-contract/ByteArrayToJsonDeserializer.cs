using Confluent.Kafka;
using System.Runtime.Serialization;
using System.Text.Json;

namespace message_contract
{
    public class ByteArrayToJsonDeserializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull) return default(T);

            try
            {
                byte[] array = data.ToArray();
                return JsonSerializer.Deserialize<T>(array);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Error deserializing JSON to the target type.", ex);
            }
        }

    }
}
