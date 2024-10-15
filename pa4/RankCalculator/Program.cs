using NATS.Client;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RankCalculator
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

    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationOptions redisOptions = ConfigurationOptions.Parse("localhost:6379");
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisOptions);
            IDatabase db = redis.GetDatabase();

            ConnectionFactory connectionFactory = new ConnectionFactory();
            IConnection natsConnection = connectionFactory.CreateConnection();

            var subscription = natsConnection.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(messageArgs.Message.Data);

                string textKey = $"TEXT-{id}";
                string textValue = db.StringGet(textKey);

                string rankKey = $"RANK-{id}";
                double rankValue = CalculateRank(textValue);

                db.StringSet(rankKey, rankValue);

                TextInfo data = new TextInfo(id, rank);
                string jsonData = JsonSerializer.Serialize(data);

                byte[] jsonDataEncoded = Encoding.UTF8.GetBytes(jsonData);

                c.Publish("valuator.logs.events.rank", jsonDataEncoded);
            });

            subscription.Start();

            Console.WriteLine("Press Enter to exit(RankCalculator)");
            Console.ReadLine();
        }

        static double CalculateRank(string text)
        {
            int totalLength = text.Length;
            int nonAlphabeticCount = 0;

            foreach (char ch in text)
            {
                if (!char.IsLetter(ch))
                {
                    nonAlphabeticCount++;
                }
            }

            return (double)nonAlphabeticCount / totalLength;
        }
    }
}