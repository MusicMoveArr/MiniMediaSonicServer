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
        return await _playlistRepository.GetPlaylistByIdAsync(playlistId);
    }

    public async Task<PlaylistModel> GetPlaylistByIdAsync(Guid playlistId)
    {
        return await _playlistRepository.GetPlaylistByIdAsync(playlistId);
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
        
        if (request.SongIdToAdd?.Any() == true)
        {
            foreach (var trackId in request.SongIdToAdd)
            {
                await _playlistRepository.AddTrackToPlaylistAsync(request.PlaylistId, trackId);
            }
            await _playlistRepository.UpdatePlaylistUpdatedAtAsync(request.PlaylistId, DateTime.Now);
        }
        
    }
}