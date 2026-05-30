using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class ScrobbleService
{
    public const string ServiceNameLastFm = "LastFm";
    public const string ServiceNameMaloja = "Maloja";
    public const string ServiceNameListenBrainz = "ListenBrainz";
    public const string ServiceNameLibreFm = "LibreFm";
    
    private readonly TrackRepository _trackRepository;
    private readonly UserPlayHistoryRepository _userPlayHistoryRepository;

    public ScrobbleService(
        TrackRepository trackRepository,
        UserPlayHistoryRepository userPlayHistoryRepository)
    {
        _trackRepository = trackRepository;
        _userPlayHistoryRepository = userPlayHistoryRepository;
    }

    public async Task ScrobbleTrackAsync(UserModel user, Guid trackId, long time)
    {
        DateTime scrobbleAt = time > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(time).DateTime : DateTime.Now;
        TrackID3? track = await _trackRepository.GetTrackByIdAsync(trackId, user.UserId);
        DateTime timeFilter = DateTime.Now - TimeSpan.FromSeconds(track.Duration);
        
        if (track == null)
        {
            return;
        }

        var userPlayHistory = await _userPlayHistoryRepository.GetLastUserPlayByTrackIdAsync(user.UserId, trackId, timeFilter);
        if (userPlayHistory == null)
        {
            await _userPlayHistoryRepository.CreatePlayHistoryAsync(user.UserId, trackId, true, scrobbleAt, time, DateTime.Now);
        }
        else
        {
            await _userPlayHistoryRepository.UpdateUserPlayHistoryAsync(userPlayHistory.HistoryId, true, scrobbleAt, DateTime.Now);
        }
    }

    public async Task PlayingNowTrackAsync(UserModel user, Guid trackId, long time)
    {
        TrackID3? track = await _trackRepository.GetTrackByIdAsync(trackId, user.UserId);
        DateTime timeFilter = DateTime.Now - TimeSpan.FromSeconds(track.Duration);

        if (track == null)
        {
            return;
        }

        var userPlayHistory = await _userPlayHistoryRepository.GetLastUserPlayByTrackIdAsync(user.UserId, trackId, timeFilter);
        if (userPlayHistory == null)
        {
            await _userPlayHistoryRepository.CreatePlayHistoryAsync(user.UserId, trackId, false, null, time, DateTime.Now);
        }
        else
        {
            await _userPlayHistoryRepository.UpdateUserPlayHistoryAsync(userPlayHistory.HistoryId, false, null, DateTime.Now);
        }
    }
}