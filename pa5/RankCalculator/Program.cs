using NATS.Client;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data.Common;

namespace RankCalculator
{
    class TextData
    {
        public TextData(string id, double data)
        {
            this.id = id;
            this.data = data;
        }
        public string id { get; set; }
        public double data { get; set; }
    }

    class IdAndCountryOfText
    {
        public IdAndCountryOfText(string country, string textId)
        {
            this.textId =   textId;
            this.country = country;
        }
        public string country { get; set; }
        public string textId { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            IConnection natsConnection = connectionFactory.CreateConnection();

            var subscription = natsConnection.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, messageArgs) =>
            {
                string receivedData = Encoding.UTF8.GetString(messageArgs.Message.Data);
                IdAndCountryInfo? info = JsonSerializer.Deserialize<IdAndCountryInfo>(receivedData);

                if (info == null) return;

                string redisEnvKey = $"DB_{info.country}";
                string? redisConnection = Environment.GetEnvironmentVariable(redisEnvKey);

                if (redisConnection == null) return;

                ConfigurationOptions redisOptions = ConfigurationOptions.Parse(redisConnection);
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisOptions);
                IDatabase db = redis.GetDatabase();

                string textKey = $"TEXT-{info.textId}";
                string textValue = db.StringGet(textKey);

                string rankKey = $"RANK-{info.textId}";
                double rankValue = CalculateRank(textValue);

                db.StringSet(rankKey, rankValue);

                TextInfo rankData = new TextInfo(info.textId, rankValue);
                string jsonData = JsonSerializer.Serialize(rankData);

                byte[] jsonEncodedData = Encoding.UTF8.GetBytes(jsonData);
                natsConnection.Publish("valuator.logs.events.rank", jsonEncodedData);

                Console.WriteLine($"LOOKUP: {info.textId}, {info.country}");
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