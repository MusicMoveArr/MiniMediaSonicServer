namespace MiniMediaSonicServer.Application.Interfaces;

public interface IRedisCacheService
{
    Task SetAsync(string prefixKey, string key, byte[] data);
    Task<byte[]?> GetAsync(string prefixKey, string key);
}