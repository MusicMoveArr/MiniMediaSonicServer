using MiniMediaSonicServer.WebJob.Indexing.Application.Repositories;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Services;

public class ReIndexSearchService
{
    private IndexedSearchRepository _indexedSearchRepository;
    public ReIndexSearchService(IndexedSearchRepository indexedSearchRepository)
    {
        _indexedSearchRepository = indexedSearchRepository;
    }

    public async Task ReIndexSearchAsync()
    {
        await _indexedSearchRepository.RemoveMissingArtistsAsync();
        await _indexedSearchRepository.RemoveMissingAlbumsAsync();
        await _indexedSearchRepository.RemoveMissingTracksAsync();
        
        //tracks
        await _indexedSearchRepository.AddMissingTracks_TitleAsync();
        await _indexedSearchRepository.AddMissingTracks_ArtistTitleAsync();
        await _indexedSearchRepository.AddMissingTracks_ArtistAlbumTitleAsync();
        //albums
        await _indexedSearchRepository.AddMissingAlbums_AlbumAsync();
        await _indexedSearchRepository.AddMissingAlbums_ArtistAlbumAsync();
        //artist
        await _indexedSearchRepository.AddMissingArtistsAsync();
    }
}