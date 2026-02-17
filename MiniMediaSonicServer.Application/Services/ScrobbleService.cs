using MiniMediaSonicServer.Application.Handlers.Scrobblers;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class ScrobbleService
{
    private readonly TrackRepository _trackRepository;
    private readonly UserPlayHistoryRepository _userPlayHistoryRepository;
    private readonly ListenBrainzScrobbleHandler _listenBrainzScrobbleHandler;
    private readonly MalojaScrobbleHandler _malojaScrobbleHandler;

    public ScrobbleService(
        TrackRepository trackRepository,
        ListenBrainzScrobbleHandler listenBrainzScrobbleHandler,
        UserPlayHistoryRepository userPlayHistoryRepository,
        MalojaScrobbleHandler malojaScrobbleHandler)
    {
        _trackRepository = trackRepository;
        _listenBrainzScrobbleHandler = listenBrainzScrobbleHandler;
        _userPlayHistoryRepository = userPlayHistoryRepository;
        _malojaScrobbleHandler = malojaScrobbleHandler;
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
        
        if (!string.IsNullOrWhiteSpace(user.ListenBrainzUserToken))
        {
            await _listenBrainzScrobbleHandler.ScrobbleAsync(track, user, scrobbleAt);
        }
        
        if (!string.IsNullOrWhiteSpace(user.MalojaUrl) && !string.IsNullOrWhiteSpace(user.MalojaApiKey))
        {
            await _malojaScrobbleHandler.ScrobbleAsync(track, user, scrobbleAt);
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