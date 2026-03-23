using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Interfaces;
using StackExchange.Redis;

namespace MiniMediaSonicServer.Application.Services;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _db;
    private readonly RedisConfiguration _redisConfiguration;

    public RedisCacheService(IConnectionMultiplexer redis, IOptions<RedisConfiguration> redisConfiguration)
    {
        _db = redis.GetDatabase();
        _redisConfiguration = redisConfiguration.Value;
    }

    public async Task SetBytesAsync(string prefixKey, string key, byte[] data)
    {
        await _db.StringSetAsync($"{prefixKey}{key}", data, _redisConfiguration.Expiry, When.Always);
    }

    public async Task<byte[]?> GetBytesAsync(string prefixKey, string key)
    {
        var value = await _db.StringGetAsync($"{prefixKey}{key}");
        if (value.HasValue)
        {
            await _db.KeyExpireAsync($"{prefixKey}{key}", _redisConfiguration.SlidingExpiration);
        }
        return value;
    }

    public async Task SetStringAsync(string prefixKey, string key, string data)
    {
        await _db.StringSetAsync($"{prefixKey}{key}", data, _redisConfiguration.Expiry, When.Always);
    }

    public async Task<string?> GetStringAsync(string prefixKey, string key)
    {
        var value = await _db.StringGetAsync($"{prefixKey}{key}");
        if (value.HasValue)
        {
            await _db.KeyExpireAsync($"{prefixKey}{key}", _redisConfiguration.SlidingExpiration);
        }
        return value;
    }
}