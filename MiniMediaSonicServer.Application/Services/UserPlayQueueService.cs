using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class UserPlayQueueService
{
    private readonly UserPlayQueueRepository _userPlayQueueRepository;
    private readonly UserService _userService;
    private readonly TrackRepository _trackRepository;

    public UserPlayQueueService(
        UserPlayQueueRepository userPlayQueueRepository,
        UserService userService,
        TrackRepository trackRepository)
    {
        _userPlayQueueRepository = userPlayQueueRepository;
        _userService = userService;
        _trackRepository = trackRepository;
    }

    public async Task SaveUserPlayQueueAsync(SavePlayQueueRequest request, Guid userId, string clientName)
    {
        DateTime datetime = await _userService.GetUserOrServerDateTimeAsync(userId);
        await _userPlayQueueRepository.UpsertUserPlayQueueAsync(
            userId, 
            request.Current, 
            request.Position,
            clientName, 
            datetime);
    }
    
    
    public async Task<UserPlayQueueModel> GetUserPlayQueueAsync(Guid userId)
    {
        var queue = await _userPlayQueueRepository.GetUserPlayQueueAsync(userId);
        if (queue?.Tracks?.Any() == true)
        {
            var tracks = await _trackRepository.GetTracksAsync(queue.Tracks.Select(t => t.TrackId).ToList(), userId);
            foreach (var track in queue.Tracks)
            {
                track.Track = tracks.FirstOrDefault(t => t.TrackId == track.TrackId);
            }
        }

        return queue;
    }


    public async Task<bool> SaveUserPlayQueueByIndexAsync(SavePlayQueueByIndexRequest request, Guid userId, string clientName)
    {
        if (request.Id?.Any() == false)
        {
            await ClearPlayQueue(userId);
            return true;
        }

        if (!request.CurrentIndex.HasValue)
        {
            request.CurrentIndex = 0;
        }
        
        var trackIds = request.Id
            .Select(id => Guid.TryParse(id, out Guid guid) ? guid : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .ToList();

        if (request.CurrentIndex < 0 || request.CurrentIndex > trackIds.Count)
        {
            return false;
        }
            
        SavePlayQueueRequest requestNonIndexed = new SavePlayQueueRequest
        {
            Id = request.Id,
            Current = trackIds[request.CurrentIndex.Value],
            Position = request.Position ?? 0,
        };
            
        await SaveUserPlayQueueAsync(requestNonIndexed, userId, clientName);
        await SaveUserPlayQueueTracksAsync(requestNonIndexed, userId);
        return true;
    }

    public async Task ClearPlayQueue(Guid userId)
    {
        DateTime datetime = await _userService.GetUserOrServerDateTimeAsync(userId);
        await _userPlayQueueRepository.UpsertUserPlayQueueAsync(
            userId, 
            null, 
            0,
            string.Empty, 
            datetime);
        await _userPlayQueueRepository.DeletePlayQueueTracksAsync(userId, 0);
    }

    public async Task SaveUserPlayQueueTracksAsync(SavePlayQueueRequest request, Guid userId)
    {
        //clear queue according to OpenSubsonic
        if (!request.Id.Any() && !
            request.Current.HasValue &&
            request.Position == 0)
        {
            await ClearPlayQueue(userId);
            return;
        }
        
        var trackIds = request.Id
            .Select(id => Guid.TryParse(id, out Guid guid) ? guid : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .ToList();
        
        int position = 0;
        DateTime datetime = await _userService.GetUserOrServerDateTimeAsync(userId);
        var currentQueue = await _userPlayQueueRepository.GetUserPlayQueueTracksAsync(userId);
        
        if (!currentQueue.Any())
        {
            //bulk insert, no records yet
            var tracks = trackIds
                .Select(trackId => new UserPlayQueueTrackModel
                {
                    UserId = userId,
                    TrackId = trackId,
                    Index = position++,
                    UpdatedAt = datetime,
                    CreatedAt = datetime
                }).ToList();
            await _userPlayQueueRepository.BulkInsertQueueTracksAsync(tracks);
        }
        else
        {
            if (currentQueue.Count != trackIds.Count)
            {
                await _userPlayQueueRepository.DeletePlayQueueTracksAsync(userId, trackIds.Count);
                
                currentQueue = currentQueue
                    .Where(track => track.Index < trackIds.Count)
                    .ToList();
            }
                
            foreach (var trackId in trackIds)
            {
                bool updateTrack = !currentQueue.Any(t => t.TrackId == trackId && t.Index == position);
                if (!updateTrack)
                {
                    position++;
                    continue;
                }

                await _userPlayQueueRepository.UpsertUserPlayQueueTrackAsync(userId, trackId, position++, datetime);
            }
        }
    }
}