using System.Security.Cryptography;
using System.Text;
using MiniMediaSonicServer.Application.Interfaces;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Handlers.Scrobblers;

public class LibreFmScrobbleHandler : IScrobble
{
    private const string LibreFmApiUrl = "https://libre.fm/2.0/";
    private readonly UserPropertyRepository _userPropertyRepository;
    public const string LibreFmApiKeySettingName = "LibreFm_ApiKey";
    public const string LibreFmApiSecretSettingName = "LibreFm_ApiSecret";
    public const string LibreFmTokenSettingName = "LibreFm_Token";

    public LibreFmScrobbleHandler(UserPropertyRepository userPropertyRepository)
    {
        _userPropertyRepository = userPropertyRepository;
    }
    
    public async Task ScrobbleAsync(TrackID3 track, UserModel user, DateTime scrobbleAt)
    {
        var unixTimeListenedAt = new DateTimeOffset(scrobbleAt).ToUnixTimeSeconds();
        string? apiKey = await _userPropertyRepository.GetUserPropertyAsync(user.UserId, LibreFmApiKeySettingName);
        string? apiSecret = await _userPropertyRepository.GetUserPropertyAsync(user.UserId, LibreFmApiSecretSettingName);
        string? token = await _userPropertyRepository.GetUserPropertyAsync(user.UserId, LibreFmTokenSettingName);

        if (string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(apiSecret) ||
            string.IsNullOrWhiteSpace(token))
        {
            return;
        }
        
        var sessionKey = await GetSessionAsync(token, apiKey, apiSecret);
        var parameters = new SortedDictionary<string, string>
        {
            ["api_key"]       = apiKey,
            ["artist[0]"]     = track.Artist,
            ["method"]        = "track.scrobble",
            ["sk"]            = sessionKey,
            ["timestamp[0]"]  = unixTimeListenedAt.ToString(),
            ["track[0]"]      = track.Title,
        };

        if (!string.IsNullOrWhiteSpace(track.Album))
        {
            parameters["album[0]"] = track.Album;
        }

        parameters["api_sig"] = GetMd5Signature(parameters, apiSecret);
        parameters["format"]  = "json";
        
        using var client = new HttpClient();
        await client.PostAsync(LibreFmApiUrl, new FormUrlEncodedContent(parameters));
    }
    
    public async Task<string?> GetTokenAsync(string apiKey, string apiSecret)
    {
        var parameters = new SortedDictionary<string, string>
        {
            ["api_key"] = apiKey,
            ["method"]  = "auth.getToken",
        };
        parameters["api_sig"] = GetMd5Signature(parameters, apiSecret);
        parameters["format"]  = "json";

        using var client = new HttpClient();
        var response = await client.PostAsync(LibreFmApiUrl, new FormUrlEncodedContent(parameters));
        var json = await response.Content.ReadAsStringAsync();

        using var doc = System.Text.Json.JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("token").GetString()!;
    }
    
    async Task<string> GetSessionAsync(string token, string apiKey, string apiSecret)
    {
        var parameters = new SortedDictionary<string, string>
        {
            ["api_key"] = apiKey,
            ["method"]  = "auth.getSession",
            ["token"]   = token,
        };
        parameters["api_sig"] = GetMd5Signature(parameters, apiSecret);
        parameters["format"]  = "json";

        using var client = new HttpClient();
        var response = await client.PostAsync(LibreFmApiUrl, new FormUrlEncodedContent(parameters));
        var json = await response.Content.ReadAsStringAsync();

        using var doc = System.Text.Json.JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("session").GetProperty("key").GetString()!;
    }
    
    private string GetMd5Signature(SortedDictionary<string, string> parameters, string secret)
    {
        var sb = new StringBuilder();
        foreach (var kv in parameters)
        {
            sb.Append(kv.Key).Append(kv.Value);
        }
        sb.Append(secret);

        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
        return Convert.ToHexString(hash).ToLower();
    }
}