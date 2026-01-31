using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class TrackService
{
    private readonly TrackRepository _trackRepository;

    public TrackService(TrackRepository trackRepository)
    {
        _trackRepository = trackRepository;
    }

    public async Task<List<TrackID3>> GetAlbumList2ResponseAsync(Guid trackId, int count)
    {
        return await _trackRepository.GetSimilarTracksAsync(trackId, count);
    }

    public async Task<List<GenreCountModel>> GetAllGenresAsync()
    {
        return await _trackRepository.GetAllGenresAsync();
    }

    public async Task<TrackID3?> GetTrackByIdAsync(Guid trackId)
    {
        return await _trackRepository.GetTrackByIdAsync(trackId);
    }

    public async Task<List<TrackID3>> GetStarredTracksAsync(Guid userId)
    {
        return await _trackRepository.GetStarredTracksAsync(userId);
    }
}