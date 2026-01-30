using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.Database;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class SearchService
{
    private readonly SearchRepository _searchRepository;

    public SearchService(SearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<List<ArtistID3>> SearchArtistsAsync(string query, int count, int offset)
    {
        return await _searchRepository.SearchArtistsAsync(query, count, offset);
    }

    public async Task<List<AlbumID3>> SearchAlbumsAsync(string query, int count, int offset)
    {
        return await _searchRepository.SearchAlbumsAsync(query, count, offset);
    }

    public async Task<List<TrackID3>> SearchTracksAsync(string query, int count, int offset)
    {
        return await _searchRepository.SearchTracksAsync(query, count, offset);
    }

    public async Task<ID3Type?> GetID3TypeAsync(Guid id)
    {
        return await _searchRepository.GetID3TypeAsync(id);
    }
}