using MiniMediaSonicServer.WebJob.Indexing.Application.Models;
using MiniMediaSonicServer.WebJob.Indexing.Application.Repositories;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Services;

public class ReIndexTrackSonic
{
    private IndexedTrackSonicRepository _indexedTrackSonicRepository;
    private int _processedTracks;
    public ReIndexTrackSonic(IndexedTrackSonicRepository indexedTrackSonicRepository)
    {
        _indexedTrackSonicRepository = indexedTrackSonicRepository;
    }

    public async Task ReIndexTrackSonicAsync()
    {
        while (true)
        {
            List<Guid> trackIds = await _indexedTrackSonicRepository.GetTrackIdsToIndexAsync();
            if (!trackIds.Any())
            {
                break;
            }
            
            Console.WriteLine($"[ReIndexTrackSonicJob][{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Found Tracks to process: {trackIds.Count}, processed: {_processedTracks}");

            foreach (var trackId in trackIds)
            {
                List<SonicTrackModel> tracks = await _indexedTrackSonicRepository.GetRelatedTracksAsync(trackId);
                if (tracks.Any())
                {
                    _processedTracks++;
                    var indexedModels = tracks
                        .Select(t => new IndexedTrackSonicModel
                        {
                            TrackId = trackId,
                            RelatedTrackId = t.TrackId,
                            Distance = t.Distance
                        })
                        .ToList();
                    await _indexedTrackSonicRepository.BulkInsertTracksAsync(indexedModels);
                }

                if (_processedTracks > 0 && _processedTracks % 10 == 0)
                {
                    Console.WriteLine($"[ReIndexTrackSonicJob][{DateTime.Now:yyyy-MM-dd HH:mm:ss}]  processed: {_processedTracks}");
                }
            }
        }
    }
}