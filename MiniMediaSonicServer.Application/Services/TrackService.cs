using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class TrackService
{
    private readonly TrackRepository _trackRepository;
    private readonly SearchRepository _searchRepository;

    public TrackService(TrackRepository trackRepository, SearchRepository searchRepository)
    {
        _trackRepository = trackRepository;
        _searchRepository = searchRepository;
    }

    public async Task<List<TrackID3>> GetAlbumList2ResponseAsync(Guid trackId, int count, Guid userId)
    {
        var id3Type = await _searchRepository.GetID3TypeAsync(trackId);
        return await _trackRepository.GetSimilarTracksAsync(trackId, count, id3Type, userId);
    }

    public async Task<List<GenreCountModel>> GetAllGenresAsync()
    {
        return await _trackRepository.GetAllGenresAsync();
    }

    public async Task<TrackID3?> GetTrackByIdAsync(Guid trackId, Guid userId)
    {
        return await _trackRepository.GetTrackByIdAsync(trackId, userId);
    }

    public async Task<List<TrackID3>> GetStarredTracksAsync(Guid userId)
    {
        return await _trackRepository.GetStarredTracksAsync(userId);
    }

    public async Task<List<TrackID3>> GetTopTracksByArtistNameAsync(string artistName, int count, Guid userId)
    {
        return await _trackRepository.GetTopArtistTracksAsync(artistName, count, userId);
    }
}