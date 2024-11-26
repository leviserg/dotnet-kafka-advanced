using Confluent.Kafka;
using message_contract;

namespace webapi_producer.Services
{
    public interface IProducerService
    {
        Task<MessageContent> SendMessageAsync(Message<string, MessageContent> message);
    }
}
