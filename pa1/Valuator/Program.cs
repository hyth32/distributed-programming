using StackExchange.Redis;

namespace Valuator;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        ConfigurationOptions redisOptions = ConfigurationOptions.Parse("localhost:6379");
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisOptions);
        builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();

        app.Run();
    }
}
