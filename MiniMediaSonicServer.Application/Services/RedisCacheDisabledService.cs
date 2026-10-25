using MiniMediaSonicServer.Application.Interfaces;

namespace MiniMediaSonicServer.Application.Services;

public class RedisCacheDisabledService : IRedisCacheService
{
    public Task SetNotFoundBytesAsync(string prefixKey, string key, byte[] data)
        => Task.CompletedTask;

    public Task SetBytesAsync(string prefixKey, string key, byte[] data)
        => Task.CompletedTask;

    public Task<byte[]?> GetBytesAsync(string prefixKey, string key) => 
        Task.FromResult<byte[]?>(null);

    public Task SetStringAsync(string prefixKey, string key, string data)
        => Task.CompletedTask;

    public Task<string?> GetStringAsync(string prefixKey, string key) => 
        Task.FromResult<string?>(null);

    public Task<bool> KeyExistsAsync(string prefixKey, string key) => 
        Task.FromResult<bool>(false);

    public Task<bool> KeyDeleteAsync(string prefixKey, string key) => 
        Task.FromResult<bool>(false);

    public Task<bool> SetExpiryAsync(string prefixKey, string key, TimeSpan expiry) => 
        Task.FromResult<bool>(false);
}