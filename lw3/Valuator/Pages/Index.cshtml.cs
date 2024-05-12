using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NATS.Client;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IDatabase _db;
    private readonly IConnection _natsConnection;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer redis, IConnection natsConnection)
    {
        _logger = logger;
        _db = redis.GetDatabase();
        _natsConnection = natsConnection;
    }

    public IActionResult OnPost(string text)
    {
        Console.WriteLine($"NATS connection state: {_natsConnection.State}");

        _logger.LogDebug(text);
        var id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;
        _db.StringSet(textKey, text);
        
        var msg = new {id, text};
        _natsConnection.Publish("text_rank_queue", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg)));
        
        string similarityKey = "SIMILARITY-" + id;
        int similarity = CalculateSimilarity(text);
        _db.StringSet(similarityKey, similarity);
        
        _db.SetAdd("texts", text);
        
        return Redirect($"summary?id={id}");
    }

    private int CalculateSimilarity(string text)
    {
        return Convert.ToInt32(_db.SetContains("texts", text));
    }
}
