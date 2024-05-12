using System.Text;
using System.Text.Json;
using NATS.Client;
using StackExchange.Redis;

public class RankCalculator : BackgroundService
{
    private readonly IDatabase _db;
    private readonly IConnection _natsConnection;

    public RankCalculator(IConnectionMultiplexer redis, IConnection natsConnection)
    {
        _db = redis.GetDatabase();
        _natsConnection = natsConnection;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subsription = _natsConnection.SubscribeAsync("text_rank_queue");
        subsription.MessageHandler += CalculateAndStoreRank;
        subsription.Start();
        
        return Task.CompletedTask;
    }

    private void CalculateAndStoreRank(object? sender, MsgHandlerEventArgs e)
    {
        var msg = JsonDocument.Parse(Encoding.UTF8.GetString(e.Message.Data)).RootElement;
        var id = msg.GetProperty("id").GetString();
        var text = msg.GetProperty("text").GetString();

        if (id != null && text != null)
        {
            string textKey = "TEXT-" + id;
            _db.StringSet(textKey, text);

            string rankKey = "RANK-" + id;
            double rank = CalculateRank(text);
            _db.StringSet(rankKey, rank);   
        }
    }

    private static double CalculateRank(string text)
    {
        int totalChars = text.Length;
        int nonAlphabetical = text.Count(c => !char.IsLetter(c));
        double rank = (double)nonAlphabetical / totalChars;
        return Math.Round(rank, 3);
    }
}