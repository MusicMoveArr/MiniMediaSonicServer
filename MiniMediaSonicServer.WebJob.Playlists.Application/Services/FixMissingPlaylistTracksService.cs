using MiniMediaSonicServer.Application.Helpers;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.WebJob.Playlists.Application.Repositories;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Services;

public class FixMissingPlaylistTracksService
{
    private readonly FixMissingPlaylistTracksRepository _fixMissingPlaylistTracksRepository;
    private readonly SearchRepository _searchRepository;
    
    public FixMissingPlaylistTracksService(
        FixMissingPlaylistTracksRepository fixMissingPlaylistTracksRepository,
        SearchRepository searchRepository)
    {
        _fixMissingPlaylistTracksRepository = fixMissingPlaylistTracksRepository;
        _searchRepository = searchRepository;
    }

    public async Task FixMissingPlaylistTracks()
    {
        var playlistTracks = await _fixMissingPlaylistTracksRepository.GetMissingPlaylistTracksAsync();

        foreach (var playlistTrack in playlistTracks)
        {
            string searchQuery = $"{playlistTrack.Artist} {playlistTrack.Album} {playlistTrack.Title}";
            var track = (await _searchRepository
                    .SearchTracksAsync(
                        searchQuery, 
                        1, 
                        0, 
                        Guid.Empty,
                        99))
                        .FirstOrDefault();
            
            bool numberingMatch = FuzzyHelper.ExactNumberMatch(searchQuery, $"{track?.Artist} {track?.Album} {track?.Title}");

            if (track != null && numberingMatch)
            {
                await _fixMissingPlaylistTracksRepository.ReplaceMissingPlaylistTrackAsync(
                    playlistTrack.TrackId, 
                    track.TrackId,
                    track.Artist,
                    track.AlbumArtists.FirstOrDefault()?.Name ?? string.Empty,
                    string.Join(';', track.Artists.Select(artist => artist.Name)),
                    track.Album,
                    track.Title,
                    string.Join(';', track.Isrc));
            }
        }
    }
}