using System.Diagnostics;
using MiniMediaSonicServer.WebJob.Indexing.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Jobs;

[DisallowConcurrentExecution]
public class FixMissingRatedTracksJob : IJob
{
    private readonly FixMissingRatedTracksService _fixMissingRatedTracksService;
    public FixMissingRatedTracksJob(FixMissingRatedTracksService fixMissingRatedTracksService)
    {
        _fixMissingRatedTracksService = fixMissingRatedTracksService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Starting FixMissingRatedTracksJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        await _fixMissingRatedTracksService.FixMissingRatedTracks();
        sw.Stop();
        Console.WriteLine($"Done FixMissingRatedTracksJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}