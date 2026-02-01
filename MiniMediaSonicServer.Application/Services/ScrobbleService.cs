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

    public ScrobbleService(
        TrackRepository trackRepository,
        ListenBrainzScrobbleHandler listenBrainzScrobbleHandler,
        UserPlayHistoryRepository userPlayHistoryRepository)
    {
        _trackRepository = trackRepository;
        _listenBrainzScrobbleHandler = listenBrainzScrobbleHandler;
        _userPlayHistoryRepository = userPlayHistoryRepository;
    }

    public async Task ScrobbleTrackAsync(UserModel user, Guid trackId, long time)
    {
        DateTime scrobbleAt = time > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(time).DateTime : DateTime.Now;
        TrackID3? track = await _trackRepository.GetTrackByIdAsync(trackId);

        if (track == null)
        {
            return;
        }

        var userPlayHistory = await _userPlayHistoryRepository.GetLastUserPlayByTrackIdAsync(user.UserId, trackId);
        if (userPlayHistory == null)
        {
            await _userPlayHistoryRepository.CreatePlayHistoryAsync(user.UserId, trackId, true, scrobbleAt);
        }
        else
        {
            await _userPlayHistoryRepository.UpdateUserPlayHistoryAsync(userPlayHistory.HistoryId, true, scrobbleAt);
        }
        
        if (!string.IsNullOrWhiteSpace(user.ListenBrainzUserToken))
        {
            await _listenBrainzScrobbleHandler.ScrobbleAsync(track, user, scrobbleAt);
        }
    }

    public async Task PlayingNowTrackAsync(UserModel user, Guid trackId, long time)
    {
        TrackID3? track = await _trackRepository.GetTrackByIdAsync(trackId);

        if (track == null)
        {
            return;
        }

        var userPlayHistory = await _userPlayHistoryRepository.GetLastUserPlayByTrackIdAsync(user.UserId, trackId);
        if (userPlayHistory == null)
        {
            await _userPlayHistoryRepository.CreatePlayHistoryAsync(user.UserId, trackId, false, null);
        }
        else
        {
            await _userPlayHistoryRepository.UpdateUserPlayHistoryAsync(userPlayHistory.HistoryId, false, null);
        }
    }
}