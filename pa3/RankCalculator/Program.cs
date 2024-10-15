using NATS.Client;
using System.Text;
using StackExchange.Redis;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationOptions redisOptions = ConfigurationOptions.Parse("localhost:6379");
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisOptions);
            IDatabase db = redis.GetDatabase();

            ConnectionFactory connectionFactory = new ConnectionFactory();
            IConnection natsConnection = connectionFactory.CreateConnection();

            var subscription = natsConnection.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, messageArgs) =>
            {
                string id = Encoding.UTF8.GetString(messageArgs.Message.Data);

                string textKey = $"TEXT-{id}";
                string textValue = db.StringGet(textKey);

                string rankKey = $"RANK-{id}";
                double rankValue = CalculateRank(textValue);

                db.StringSet(rankKey, rankValue);
            });

            subscription.Start();

            Console.WriteLine("Press Enter to exit");
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