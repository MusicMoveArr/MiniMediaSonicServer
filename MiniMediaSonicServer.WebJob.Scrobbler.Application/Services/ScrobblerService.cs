using Microsoft.AspNetCore.Identity;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.WebJob.Scrobbler.Application.Handlers.Scrobblers;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.WebJob.Scrobbler.Application.Services;

public class ScrobblerService
{
    private readonly ListenBrainzScrobbleHandler _listenBrainzScrobbleHandler;
    private readonly MalojaScrobbleHandler _malojaScrobbleHandler;
    private readonly LibreFmScrobbleHandler _libreFmScrobbleHandler;
    private readonly UserRepository _userRepository;
    private readonly UserPlayHistoryRepository _userPlayHistoryRepository;
    private readonly TrackRepository _trackRepository;
    private const int MaxFailSendCount = 5;
    
    public ScrobblerService(
        TrackRepository trackRepository,
        ListenBrainzScrobbleHandler listenBrainzScrobbleHandler,
        UserPlayHistoryRepository userPlayHistoryRepository,
        MalojaScrobbleHandler malojaScrobbleHandler,
        LibreFmScrobbleHandler libreFmScrobbleHandler,
        UserRepository userRepository)
    {
        _listenBrainzScrobbleHandler = listenBrainzScrobbleHandler;
        _malojaScrobbleHandler = malojaScrobbleHandler;
        _libreFmScrobbleHandler = libreFmScrobbleHandler;
        _userRepository = userRepository;
        _userPlayHistoryRepository = userPlayHistoryRepository;
        _trackRepository = trackRepository;
    }

    public async Task SendAllUserScrobblesAsync()
    {
        var userIds = await _userRepository.GetAllUserIdsAsync();
        foreach (var userId in userIds.AsParallel())
        {
            try
            {
                await SendUserScrobblesAsync(userId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
    
    public async Task SendUserScrobblesAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user != null)
        {
            var lbScrobbles = SendListenBrainzScrobbles(user);
            var mlScrobbles = SendMalojaScrobbles(user);
            var libreFmScrobbles = SendLibreFmScrobbles(user);
            await Task.WhenAll(lbScrobbles, mlScrobbles, libreFmScrobbles);
        }
    }

    private async Task SendLibreFmScrobbles(UserModel user)
    {
        var notSendScrobbles = await _userPlayHistoryRepository
            .GetUserPlayHistoryNotSendAsync(user.UserId, ScrobbleService.ServiceNameLibreFm, 1000);
        var tracks = await _trackRepository
            .GetTracksAsync(notSendScrobbles.Select(t => t.TrackId).ToList(), user.UserId);

        int failCount = 0;
        foreach (var scrobble in notSendScrobbles)
        {
            var track =  tracks.FirstOrDefault(t => t.TrackId == scrobble.TrackId);
            if (track != null)
            {
                bool success = await _libreFmScrobbleHandler.ScrobbleAsync(track, user, scrobble.ScrobbleAt.Value);
                if (success)
                {
                    await _userPlayHistoryRepository.InsertPlayHistorySendAsync(scrobble.HistoryId, ScrobbleService.ServiceNameLibreFm);
                }
                else
                {
                    failCount++;
                }

                if (failCount > MaxFailSendCount)
                {
                    break;
                }
            }
        }
    }

    private async Task SendMalojaScrobbles(UserModel user)
    {
        if (string.IsNullOrWhiteSpace(user.MalojaUrl) || string.IsNullOrWhiteSpace(user.MalojaApiKey))
        {
            return;
        }

        var notSendScrobbles = await _userPlayHistoryRepository
            .GetUserPlayHistoryNotSendAsync(user.UserId, ScrobbleService.ServiceNameMaloja, 1000);
        var tracks = await _trackRepository
            .GetTracksAsync(notSendScrobbles.Select(t => t.TrackId).ToList(), user.UserId);

        int failCount = 0;
        foreach (var scrobble in notSendScrobbles)
        {
            var track =  tracks.FirstOrDefault(t => t.TrackId == scrobble.TrackId);
            if (track != null)
            {
                bool success = await _malojaScrobbleHandler.ScrobbleAsync(track, user, scrobble.ScrobbleAt.Value);
                if (success)
                {
                    await _userPlayHistoryRepository.InsertPlayHistorySendAsync(scrobble.HistoryId, ScrobbleService.ServiceNameMaloja);
                }
                else
                {
                    failCount++;
                }

                if (failCount > MaxFailSendCount)
                {
                    break;
                }
            }
        }
    }

    private async Task SendListenBrainzScrobbles(UserModel user)
    {
        if (string.IsNullOrWhiteSpace(user.ListenBrainzUserToken))
        {
            return;
        }

        var notSendScrobbles = await _userPlayHistoryRepository
            .GetUserPlayHistoryNotSendAsync(user.UserId, ScrobbleService.ServiceNameListenBrainz, 1000);
        var tracks = await _trackRepository
            .GetTracksAsync(notSendScrobbles.Select(t => t.TrackId).ToList(), user.UserId);

        int failCount = 0;
        foreach (var scrobble in notSendScrobbles)
        {
            var track =  tracks.FirstOrDefault(t => t.TrackId == scrobble.TrackId);
            if (track != null)
            {
                bool success = await _listenBrainzScrobbleHandler.ScrobbleAsync(track, user, scrobble.ScrobbleAt.Value);
                if (success)
                {
                    await _userPlayHistoryRepository.InsertPlayHistorySendAsync(scrobble.HistoryId, ScrobbleService.ServiceNameListenBrainz);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                else
                {
                    failCount++;
                }

                if (failCount > MaxFailSendCount)
                {
                    break;
                }
            }
        }
    }
}