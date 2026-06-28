using System.Diagnostics;
using MiniMediaSonicServer.WebJob.Indexing.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Jobs;

[DisallowConcurrentExecution]
public class ReIndexTrackSonicJob : IJob
{
    private readonly ReIndexTrackSonic _reIndexTrackSonic;
    public ReIndexTrackSonicJob(ReIndexTrackSonic reIndexTrackSonic)
    {
        _reIndexTrackSonic = reIndexTrackSonic;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Starting ReIndexTrackSonicJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        await _reIndexTrackSonic.ReIndexTrackSonicAsync();
        sw.Stop();
        Console.WriteLine($"Done ReIndexTrackSonicJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}