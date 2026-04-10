using System.Diagnostics;
using MiniMediaSonicServer.WebJob.Playlists.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Jobs;

[DisallowConcurrentExecution]
public class FixMissingPlaylistTracksJob : IJob
{
    private readonly FixMissingPlaylistTracksService _fixMissingPlaylistTracksService;
    public FixMissingPlaylistTracksJob(FixMissingPlaylistTracksService fixMissingPlaylistTracksService)
    {
        _fixMissingPlaylistTracksService = fixMissingPlaylistTracksService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Starting FixMissingPlaylistTracksJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        await _fixMissingPlaylistTracksService.FixMissingPlaylistTracks();
        sw.Stop();
        Console.WriteLine($"Done FixMissingPlaylistTracksJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}