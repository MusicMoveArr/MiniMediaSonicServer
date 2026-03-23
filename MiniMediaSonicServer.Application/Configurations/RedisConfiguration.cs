namespace MiniMediaSonicServer.Application.Configurations;

public class RedisConfiguration
{
    public string ConnectionString { get; set; }
    public TimeSpan Expiry { get; set; } = TimeSpan.FromDays(1);
    public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromDays(1);
}