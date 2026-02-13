namespace MiniMediaSonicServer.Application.Interfaces;

public interface IRedisCacheService
{
    Task SetBytesAsync(string prefixKey, string key, byte[] data);
    Task<byte[]?> GetBytesAsync(string prefixKey, string key);
    
    Task SetStringAsync(string prefixKey, string key, string data);
    Task<string?> GetStringAsync(string prefixKey, string key);
}