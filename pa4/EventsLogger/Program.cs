using NATS.Client;
using System.Text;
using System.Text.Json;

namespace EventsLogger
{
    class TextInfo
    {
        public TextInfo(string id, double data)
        {
            this.id = id;
            this.data = data;
        }
        public string id { get; set; }
        public double data { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var connectionFactory = new ConnectionFactory();
            var natsConnection = connectionFactory.CreateConnection();

            var rankSubscription = natsConnection.SubscribeAsync("valuator.logs.events.rank", "events_logger", (sender, messageArgs) =>
            {
                string receivedData = Encoding.UTF8.GetString(messageArgs.Message.Data);
                var textData = JsonSerializer.Deserialize<TextInfo>(receivedData);
                Console.WriteLine($"Event Type: Rank\nID: {textData?.RecordId}\nValue: {textData?.Value}");
                
            });
            rankSubscription.Start();

            var similaritySubscription = natsConnection.SubscribeAsync("valuator.logs.events.similarity", "events_logger", (sender, messageArgs) =>
            {
                string receivedData = Encoding.UTF8.GetString(messageArgs.Message.Data);
                var textData = JsonSerializer.Deserialize<TextInfo>(receivedData);
                Console.WriteLine($"Event Type: Similarity\nID: {textData?.RecordId}\nValue: {textData?.Value}");
            });
            similaritySubscription.Start();

            Console.WriteLine("Press Enter to exit (EventsLogger)");
            Console.ReadLine();
        }
    }

}