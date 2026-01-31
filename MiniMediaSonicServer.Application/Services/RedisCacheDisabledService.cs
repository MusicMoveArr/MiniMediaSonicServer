using MiniMediaSonicServer.Application.Interfaces;

namespace MiniMediaSonicServer.Application.Services;

public class RedisCacheDisabledService : IRedisCacheService
{
    public Task SetAsync(string prefixKey, string key, byte[] data)
        => Task.CompletedTask;

    public Task<byte[]?> GetAsync(string prefixKey, string key) => 
        Task.FromResult<byte[]?>(null);
}