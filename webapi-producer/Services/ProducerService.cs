﻿using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
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
            var schemaRegistryConfig = new SchemaRegistryConfig
            {
                Url = "http://localhost:8081"  // URL of your Schema Registry
            };

            /* optional
            var jsonSerializerConfig = new JsonSerializerConfig
            {
                BufferBytes = 100
            };
            */
            var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
            var jsonSerializer = new JsonSerializer<MessageContent>(schemaRegistry).AsSyncOverAsync();

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

                EnableDeliveryReports = true, // false = fire & forget
                Acks = Acks.All, // None - 0, Leader - 1
                /*
                 * Other options setting:
                CompressionType = CompressionType.Gzip,
                CompressionLevel = 2, // 0-12, def -1 - better compress - higher cpuload
                LingerMs = 3,
                */
            };

            var producer = new ProducerBuilder<string, MessageContent>(config)
                .SetValueSerializer(jsonSerializer)
                .Build();

            try
            {

                var result = await producer.ProduceAsync(TOPIC, message);

                /* synchronous mode
                producer.Produce(TOPIC, message,
                (deliveryReport) => // this is callbback option
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
                */

                producer.Flush(TimeSpan.FromSeconds(5));

                return result.Value;

            }
            catch (Exception ex) // ProduceException
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                schemaRegistry.Dispose();
                producer.Dispose();
            }
        }
    }
}
