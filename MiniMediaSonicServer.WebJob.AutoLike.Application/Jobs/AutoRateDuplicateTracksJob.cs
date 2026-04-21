using System.Diagnostics;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Jobs;

[DisallowConcurrentExecution]
public class AutoRateDuplicateTracksJob : IJob
{
    private readonly AutoRateDuplicateTracksService _autoRateDuplicateTracksService;
    public AutoRateDuplicateTracksJob(AutoRateDuplicateTracksService autoRateDuplicateTracksService)
    {
        _autoRateDuplicateTracksService = autoRateDuplicateTracksService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Starting AutoRateDuplicateTracksJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        await _autoRateDuplicateTracksService.RateAlbumsAsync();
        sw.Stop();
        Console.WriteLine($"Done AutoRateDuplicateTracksJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}