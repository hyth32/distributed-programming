using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;

namespace Valuator.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IDatabase _db;

    public IndexModel(ILogger<IndexModel> logger, IConnectionMultiplexer redis)
    {
        _logger = logger;
        _db = redis.GetDatabase();
    }

    public IActionResult OnPost(string text)
    {
        _logger.LogDebug(text);
        var id = Guid.NewGuid().ToString();

        string textKey = "TEXT-" + id;
        _db.StringSet(textKey, text);
        
        string rankKey = "RANK-" + id;
        double rank = CalculateRank(text);
        _db.StringSet(rankKey, rank);
        
        string similarityKey = "SIMILARITY-" + id;
        int similarity = CalculateSimilarity(text);
        _db.StringSet(similarityKey, similarity);
        
        _db.SetAdd("texts", text);
        
        return Redirect($"summary?id={id}");
    }

    private static double CalculateRank(string text)
    {
        int totalChars = text.Length;
        int nonAlphabetical = text.Count(c => !char.IsLetter(c));
        double rank = (double)nonAlphabetical / totalChars;
        return Math.Round(rank, 3);
    }

    private int CalculateSimilarity(string text)
    {
        return Convert.ToInt32(_db.SetContains("texts", text));
    }
}
