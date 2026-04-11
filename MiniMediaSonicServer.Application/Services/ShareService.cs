using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class ShareService
{
    private const int ShareLength = 10;
    private static readonly List<char> _shareCharacters = 
        Enumerable.Range(48, 9) //0-9
        .Union(Enumerable.Range(65, 25)) //A-Z
        .Union(Enumerable.Range(97, 25)) //a-z
        .Select(i => (char)i)
        .ToList();
    
    private readonly AlbumService _albumService;
    private readonly ShareRepository _shareRepository;
    private readonly SearchRepository _searchRepository;
    private readonly TrackService _trackService;
    private readonly Random _random = new Random();

    public ShareService(ShareRepository shareRepository, 
        SearchRepository searchRepository,
        AlbumService albumService,
        TrackService trackService)
    {
        _shareRepository = shareRepository;
        _searchRepository = searchRepository;
        _albumService = albumService;
        _trackService = trackService;
    }

    public async Task<string> CreateShareAsync(Guid trackId, Guid userId, string? description, long? expiresAt)
    {
        var id3Type = await _searchRepository.GetID3TypeAsync(trackId);
        string shareName = GetShareName();
        
        DateTime? expireAt = expiresAt.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(expiresAt.Value).DateTime : null;
        await _shareRepository.CreateShareAsync(userId, shareName, description, expireAt, id3Type.ToString(), trackId);
        return shareName;
    }

    public async Task<bool> IsExpiredAsync(ShareModel share)
    {
        if (!share.ExpiresAt.HasValue)
        {
            return false;
        }
        
        return DateTime.Now > share.ExpiresAt.Value;
    }

    public async Task<ShareModel?> GetShareAsync(string shareName)
    {
        return await _shareRepository.GetShareAsync(shareName);
    }

    public async Task<List<ShareModel>> GetAllSharesAsync(Guid userId)
    {
        return await _shareRepository.GetAllSharesAsync(userId);
    }

    public async Task IncrementVisitorCountAsync(Guid shareId)
    {
        await _shareRepository.IncrementVisitorCountAsync(shareId);
    }

    public async Task<List<Guid>> GetPlayableTrackIdsAsync(string shareName)
    {
        List<Guid> trackIds = new List<Guid>();
        var share = await GetShareAsync(shareName);

        if (share == null)
        {
            return new List<Guid>();
        }

        if (share.Type == nameof(ID3Type.Track))
        {
            return [share.MediaId];
        }
        return (await _albumService.GetAlbumByIdResponseAsync(share.MediaId, Guid.NewGuid()))
            ?.Song
            ?.Select(song => song.TrackId)
            ?.ToList() ?? [];
    }

    public async Task<List<TrackID3>> GetSharedTrackAsync(string shareName)
    {
        List<Guid> trackIds = await GetPlayableTrackIdsAsync(shareName);
        return await _trackService.GetTrackByIdAsync(trackIds, Guid.NewGuid());
    }

    public async Task DeleteShareAsync(Guid userId, Guid shareId)
    {
        await _shareRepository.DeleteShareAsync(userId, shareId);
    }
    
    private string GetShareName()
    {
        return string.Concat(Enumerable.Range(0, ShareLength)
            .Select(c => _random.Next(0, _shareCharacters.Count - 1))
            .Select(c => _shareCharacters[c]));
    }
}