using MiniMediaSonicServer.Application.Interfaces;
using StackExchange.Redis;

namespace MiniMediaSonicServer.Application.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task SetAsync(string prefixKey, string key, byte[] data)
    {
        await _db.StringSetAsync($"{prefixKey}{key}", data, TimeSpan.FromHours(1), When.Always);
    }

    public async Task<byte[]?> GetAsync(string prefixKey, string key)
    {
        return await _db.StringGetAsync($"{prefixKey}{key}");
    }
}