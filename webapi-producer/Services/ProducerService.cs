using Confluent.Kafka;
using message_contract;
using System.Diagnostics;

namespace webapi_producer.Services
{
    public class ProducerService : IProducerService
    {
        const string TOPIC = "kafka.learning.orders";

        private readonly ILogger<ProducerService> _logger;

        public ProducerService(ILogger<ProducerService> logger)
        {
            _logger = logger;
        }

        public async Task<MessageContent> SendMessageAsync(Message<string, MessageContent> message)
        {
            ProducerConfig config = new ProducerConfig
            {
                // User-specific properties that you must set
                BootstrapServers = "localhost:9092,localhost:9093,localhost:9094",

                // exact once
                EnableIdempotence = true,
                MaxInFlight = 5,
                MessageSendMaxRetries = 3,

                //SaslUsername = "<SSAL_USERNAME>",
                //SaslPassword = "<SSAL_PASSWORD>",

                // Fixed properties
                SecurityProtocol = SecurityProtocol.Plaintext,//SaslSsl, //Plaintext,SaslPlaintext
                //SaslMechanism = SaslMechanism.Plain,
                Acks = Acks.All
            };

            var producer = new ProducerBuilder<string, MessageContent>(config)
                .SetValueSerializer(new JsonToByteArraySerializer<MessageContent>())
                .Build();

            try
            {

                producer.Produce(TOPIC, message,
                (deliveryReport) =>
                {
                    if (deliveryReport.Error.IsError)
                    {
                        throw new ProduceException<string, MessageContent>(deliveryReport.Error, deliveryReport);
                    }
                    else
                    {
                        string producedMessage = deliveryReport.Key + "\t" + deliveryReport.Value + "\t" + deliveryReport.Timestamp;
                        _logger.LogInformation(producedMessage);
                    }
                });

                producer.Flush(TimeSpan.FromSeconds(20));

                return await Task.FromResult(message.Value);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                producer.Dispose();
            }
        }
    }
}
