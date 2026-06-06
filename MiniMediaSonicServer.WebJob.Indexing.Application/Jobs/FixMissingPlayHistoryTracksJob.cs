using System.Diagnostics;
using MiniMediaSonicServer.WebJob.Indexing.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Jobs;

[DisallowConcurrentExecution]
public class FixMissingPlayHistoryTracksJob : IJob
{
    private readonly FixMissingPlayHistoryTracksService _fixMissingPlayHistoryTracksService;
    public FixMissingPlayHistoryTracksJob(FixMissingPlayHistoryTracksService fixMissingPlayHistoryTracksService)
    {
        _fixMissingPlayHistoryTracksService = fixMissingPlayHistoryTracksService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Starting FixMissingPlayHistoryTracksJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        await _fixMissingPlayHistoryTracksService.FixMissingPlayHistoryTracks();
        sw.Stop();
        Console.WriteLine($"Done FixMissingPlayHistoryTracksJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}