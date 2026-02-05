using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class SearchService
{
    private readonly SearchRepository _searchRepository;
    private readonly SearchSyncRepository _searchSyncRepository;

    public SearchService(
        SearchRepository searchRepository,
        SearchSyncRepository searchSyncRepository)
    {
        _searchRepository = searchRepository;
        _searchSyncRepository = searchSyncRepository;
    }

    public async Task<List<ArtistID3>> SearchArtistsAsync(string query, int count, int offset)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await _searchSyncRepository.SearchArtistsAsync(count, offset);
        }
        return await _searchRepository.SearchArtistsAsync(query, count, offset);
    }

    public async Task<List<AlbumID3>> SearchAlbumsAsync(string query, int count, int offset, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await _searchSyncRepository.SearchAlbumsAsync(count, offset, userId);
        }
        return await _searchRepository.SearchAlbumsAsync(query, count, offset, userId);
    }

    public async Task<List<TrackID3>> SearchTracksAsync(string query, int count, int offset)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return await _searchSyncRepository.SearchTracksAsync(count, offset);
        }
        return await _searchRepository.SearchTracksAsync(query, count, offset);
    }

    public async Task<ID3Type?> GetID3TypeAsync(Guid id)
    {
        return await _searchRepository.GetID3TypeAsync(id);
    }
}