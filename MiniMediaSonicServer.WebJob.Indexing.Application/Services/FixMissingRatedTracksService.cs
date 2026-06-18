using MiniMediaSonicServer.Application.Helpers;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.WebJob.Indexing.Application.Repositories;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Services;

public class FixMissingRatedTracksService
{
    private readonly FixMissingRatedTracksRepository _fixMissingRatedTracksRepository;
    private readonly SearchRepository _searchRepository;
    
    public FixMissingRatedTracksService(
        FixMissingRatedTracksRepository fixMissingRatedTracksRepository,
        SearchRepository searchRepository)
    {
        _fixMissingRatedTracksRepository = fixMissingRatedTracksRepository;
        _searchRepository = searchRepository;
    }

    public async Task FixMissingRatedTracks()
    {
        var ratedTracks = await _fixMissingRatedTracksRepository.GetMissingRatedTracksAsync();

        foreach (var ratedTrack in ratedTracks)
        {
            string searchQuery = $"{ratedTrack.Artist} {ratedTrack.Album} {ratedTrack.Title}";
            
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
                await _fixMissingRatedTracksRepository.ReplaceMissingRatedTrackAsync(
                    ratedTrack.TrackId, 
                    track.TrackId,
                    track.Artist,
                    track.AlbumArtists.FirstOrDefault()?.Name ?? string.Empty,
                    string.Join(';', track.Artists.Select(artist => artist.Name)),
                    track.Album,
                    track.Title,
                    string.Join(';', track.Isrc));
            }
            else
            {
                Console.WriteLine($"FixMissingRatedTracksJob, Missing track: {ratedTrack.Artist} {ratedTrack.Album} {ratedTrack.Title}");
            }
        }
    }
}