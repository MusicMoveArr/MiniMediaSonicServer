using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using MiniMediaSonicServer.Application.Interfaces;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.WebJob.Import.Application.Services;

public class ImportLastFmScrobblesService
{
    private readonly SearchRepository _searchRepository;
    private readonly UserRepository _userRepository;
    private readonly UserPropertyRepository _userPropertyRepository;
    private readonly UserPlayHistoryRepository _userPlayHistoryRepository;
    private readonly IRedisCacheService _redisCacheService;
    
    public ImportLastFmScrobblesService(
        SearchRepository searchRepository,
        UserRepository userRepository,
        UserPropertyRepository userPropertyRepository,
        UserPlayHistoryRepository userPlayHistoryRepository,
        IRedisCacheService redisCacheService)
    {
        _searchRepository = searchRepository;
        _userRepository = userRepository;
        _userPropertyRepository = userPropertyRepository;
        _userPlayHistoryRepository = userPlayHistoryRepository;
        _redisCacheService = redisCacheService;
    }

    public async Task SyncAllUserScrobblesAsync()
    {
        var userIds = await _userRepository.GetAllUserIdsAsync();
        foreach (var userId in userIds)
        {
            await SyncUserScrobblesAsync(userId);
        }
    }

    public async Task SyncUserScrobblesAsync(Guid userId)
    {
        string? lastfmApiKey = await _userPropertyRepository.GetUserPropertyAsync(userId, "LASTFM_APIKEY");
        string? lastfmSharedSecret = await _userPropertyRepository.GetUserPropertyAsync(userId, "LASTFM_APISECRET");
        string? lastfmUser = await _userPropertyRepository.GetUserPropertyAsync(userId, "LASTFM_USER");

        if (string.IsNullOrEmpty(lastfmApiKey) || 
            string.IsNullOrEmpty(lastfmSharedSecret) || 
            string.IsNullOrEmpty(lastfmUser))
        {
            return;
        }
        
        TimeZoneInfo? userTimezone = await _userRepository.GetTimezoneByUserIdAsync(userId);
        if (userTimezone == null)
        {
            return;
        }
        
        LastfmClient client = new LastfmClient(lastfmApiKey, lastfmSharedSecret);
        for (int page = 1;; page++)
        {
            var scrobbles = await client.User.GetRecentScrobbles(
                lastfmUser, 
                pagenumber: page, 
                count: 500, 
                extendedResponse: false);

            if (!scrobbles.Any())
            {
                break;
            }
            
            foreach (var lastfmTrack in scrobbles.Content.Where(t => t.TimePlayed.HasValue))
            {
                await ProcessScrobble(lastfmTrack, userTimezone, userId);
            }
        }
    }

    private async Task ProcessScrobble(
        LastTrack lastfmTrack, 
        TimeZoneInfo userTimezone,
        Guid userId)
    {
        long epoch = lastfmTrack.TimePlayed.Value.ToUnixTimeSeconds();
        var scrobbleAt = lastfmTrack.TimePlayed.Value
            .DateTime.Add(userTimezone.BaseUtcOffset);
            
        var scrobbled = await _userPlayHistoryRepository
            .GetScrobbledAtTimeAsync(userId, scrobbleAt, epoch);

        if(scrobbled.Any())
        {
            return;
        }

        string searchQuery = $"{lastfmTrack.ArtistName} {lastfmTrack.AlbumName} {lastfmTrack.Name}";
        string searchQueryRedisKey = $"ProcessScrobble_Track_{searchQuery}";
        var cachedTrackId = await _redisCacheService.GetStringAsync(string.Empty, searchQueryRedisKey);

        if (!Guid.TryParse(cachedTrackId, out Guid trackId))
        {
            var track = (await _searchRepository
                    .SearchTracksAsync(searchQuery, 
                        1, 
                        0, 
                        99))
                .FirstOrDefault();
            if (track != null && trackId != Guid.Empty)
            {
                trackId = track.TrackId;
                await _redisCacheService.SetStringAsync(string.Empty, searchQueryRedisKey, trackId.ToString());
            }
        }
        
        if (trackId != null && trackId != Guid.Empty)
        {
            await _userPlayHistoryRepository.CreatePlayHistoryAsync(
                userId, trackId, 
                true,
                scrobbleAt, 
                epoch,
                scrobbleAt,
                "Last.fm");
        }
    }
}