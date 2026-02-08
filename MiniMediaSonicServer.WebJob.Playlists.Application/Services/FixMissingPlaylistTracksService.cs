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
            var track = (await _searchRepository
                    .SearchTracksAsync(
                    $"{playlistTrack.Artist} {playlistTrack.Album} {playlistTrack.Title}", 
                    1, 
                    0, 
                    99))
                    .FirstOrDefault();

            if (track != null)
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