using MiniMediaSonicServer.Application.Handlers.Scrobblers;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class ScrobbleService
{
    private readonly TrackRepository _trackRepository;
    private readonly ListenBrainzScrobbleHandler _listenBrainzScrobbleHandler;

    public ScrobbleService(
        TrackRepository trackRepository,
        ListenBrainzScrobbleHandler listenBrainzScrobbleHandler)
    {
        _trackRepository = trackRepository;
        _listenBrainzScrobbleHandler = listenBrainzScrobbleHandler;
    }

    public async Task ScrobbleTrackAsync(UserModel user, Guid trackId, long time)
    {
        DateTime scrobbleAt = time > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(time).DateTime : DateTime.Now;
        TrackID3? track = await _trackRepository.GetTrackByIdAsync(trackId);

        if (track == null)
        {
            return;
        }
        
        if (!string.IsNullOrWhiteSpace(user.ListenBrainzUserToken))
        {
            await _listenBrainzScrobbleHandler.ScrobbleAsync(track, user, scrobbleAt);
        }
    }
}