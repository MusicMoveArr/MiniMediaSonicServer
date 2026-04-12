using System.Diagnostics;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Jobs;

[DisallowConcurrentExecution]
public class AutoRateAlbumsJob : IJob
{
    private readonly AutoRateAlbumsService _autoRateAlbumsService;
    public AutoRateAlbumsJob(AutoRateAlbumsService autoRateAlbumsService)
    {
        _autoRateAlbumsService = autoRateAlbumsService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Starting AutoRateAlbumsJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        await _autoRateAlbumsService.RateAlbumsAsync();
        sw.Stop();
        Console.WriteLine($"Done AutoRateAlbumsJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}