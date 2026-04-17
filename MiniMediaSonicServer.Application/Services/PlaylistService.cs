using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class PlaylistService
{
    private readonly PlaylistRepository _playlistRepository;
    private readonly string[] PlaylistTrackColumns = 
    [
        "PlaylistId",
        "TrackId",
        "TrackOrder",
        "CreatedAt"
    ];

    public PlaylistService(PlaylistRepository playlistRepository)
    {
        _playlistRepository = playlistRepository;
    }

    public async Task<List<PlaylistModel>> GetAllPlaylistsAsync(Guid userId)
    {
        return await _playlistRepository.GetAllPlaylistsAsync(userId);
    }

    public async Task<PlaylistModel> CreatePlaylistAsync(Guid userId, string name)
    {
        Guid playlistId = await _playlistRepository.CreatePlaylistAsync(userId, name);
        return await _playlistRepository.GetPlaylistByIdAsync(playlistId, userId);
    }

    public async Task<PlaylistModel?> GetPlaylistByIdAsync(Guid playlistId, Guid userId)
    {
        return await _playlistRepository.GetPlaylistByIdAsync(playlistId, userId);
    }

    public async Task SetPlaylistDeletedAsync(Guid playlistId)
    {
        await _playlistRepository.SetPlaylistDeletedAsync(playlistId);
    }

    public async Task UpdatePlaylistByIdAsync(UpdatePlaylistRequest request)
    {
        if (!await _playlistRepository.PlaylistExistsAsync(request.PlaylistId))
        {
            return;
        }
        
        if (!string.IsNullOrWhiteSpace(request.SongIndexToRemove))
        {
            int[] indexes = request.SongIndexToRemove
                .Split(',')
                .Select(index => int.TryParse(index, out int i) ? i : -1)
                .Where(index => index >= 0)
                .OrderByDescending(index => index)
                .ToArray();
            
            foreach (var index in indexes)
            {
                await _playlistRepository.RemoveTrackAtIndexAsync(request.PlaylistId, index);
            }
            await _playlistRepository.UpdatePlaylistUpdatedAtAsync(request.PlaylistId, DateTime.Now);
        }
        
        if (request.SongIdToAdd?.Any() == true)
        {
            foreach (var trackId in request.SongIdToAdd)
            {
                await _playlistRepository.AddTrackToPlaylistAsync(request.PlaylistId, trackId);
            }
            await _playlistRepository.UpdatePlaylistUpdatedAtAsync(request.PlaylistId, DateTime.Now);
        }
    }

    public async Task<List<Guid>> GetPlaylistTrackIdsAsync(Guid playlistId)
    {
        return await _playlistRepository.GetPlaylistTrackIdsAsync(playlistId);
    }
}