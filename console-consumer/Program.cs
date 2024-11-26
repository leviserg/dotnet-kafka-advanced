using Confluent.Kafka;
using message_contract;

Console.WriteLine("Enter topic you want me to consume or press ENTER to use default topic:");
const string defaultTopic = "kafka.learning.orders";
string? selectedTopic = Console.ReadLine();
string topic = string.IsNullOrEmpty(selectedTopic) ? defaultTopic : selectedTopic;

Console.WriteLine($"Subscribed to topic: {topic}");

var conf = new ConsumerConfig
{
    GroupId = "kafka-net-consumer",//"test-consumer-group",
    BootstrapServers = "localhost:9092,localhost:9093,localhost:9094",
    AutoOffsetReset = AutoOffsetReset.Earliest
};


var consumer = new ConsumerBuilder<string, MessageContent>(conf)
    .SetValueDeserializer(new ByteArrayToJsonDeserializer<MessageContent>())
    .Build();

consumer.Subscribe(topic); // can be IEnumerable<string>

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => {
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

try
{
    Console.WriteLine($"Waiting for messages...");
    while (true)
    {
        try
        {
            var receivedMessage = consumer.Consume(cts.Token);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{receivedMessage.Message.Timestamp.UtcDateTime.ToString("HH:mm:ss")} : '{receivedMessage.Message.Value.ToString()}' at: '{receivedMessage.TopicPartitionOffset}'.");
            if (cts.IsCancellationRequested)
            {
                Console.WriteLine($"Cancel request received");
                consumer.Unsubscribe();
                consumer.Close();
                Console.WriteLine($"Closed");
                break;
            }
        }
        catch (ConsumeException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error occurred: {e.Error.Reason}");
            Console.ResetColor();
        }
    }
}
catch (OperationCanceledException)
{
    // Ensure the consumer leaves the group cleanly and final offsets are committed.
    Console.WriteLine($"Closing consumer...");
    consumer.Close();
    Console.WriteLine($"Closed");
}
finally
{
    Console.WriteLine($"Disposing...");
    consumer.Dispose();
    Console.WriteLine($"Disposed");
}

Console.ReadLine();
