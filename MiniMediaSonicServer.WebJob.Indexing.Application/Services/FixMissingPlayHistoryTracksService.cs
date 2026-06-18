using MiniMediaSonicServer.Application.Helpers;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.WebJob.Indexing.Application.Repositories;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Services;

public class FixMissingPlayHistoryTracksService
{
    private readonly FixMissingPlayHistoryTracksRepository _fixMissingPlayHistoryTracksRepository;
    private readonly SearchRepository _searchRepository;
    
    public FixMissingPlayHistoryTracksService(
        FixMissingPlayHistoryTracksRepository fixMissingPlayHistoryTracksRepository,
        SearchRepository searchRepository)
    {
        _fixMissingPlayHistoryTracksRepository = fixMissingPlayHistoryTracksRepository;
        _searchRepository = searchRepository;
    }

    public async Task FixMissingPlayHistoryTracks()
    {
        var historyTracks = await _fixMissingPlayHistoryTracksRepository.GetMissingPlayHistoryTracksAsync();

        foreach (var historyTrack in historyTracks)
        {
            string searchQuery = $"{historyTrack.Artist} {historyTrack.Album} {historyTrack.Title}";
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
                await _fixMissingPlayHistoryTracksRepository.ReplaceMissingPlayHistoryAsync(
                    historyTrack.HistoryId,
                    historyTrack.TrackId, 
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
                Console.WriteLine($"FixMissingPlayHistoryTracksJob, Missing track: {historyTrack.Artist} {historyTrack.Album} {historyTrack.Title}");
            }
        }
    }
}