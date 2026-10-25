namespace MiniMediaSonicServer.Application.Configurations;

public class RedisConfiguration
{
    public string? ConnectionString { get; set; }
    public TimeSpan Expiry { get; set; } = TimeSpan.FromDays(1);
    public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromDays(1);
    public TimeSpan NotFoundExpiry { get; set; } = TimeSpan.FromMinutes(15);
    public TimeSpan NotFoundSlidingExpiration { get; set; } = TimeSpan.FromMinutes(15);
}