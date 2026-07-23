using System.Diagnostics;
using Microsoft.Extensions.Logging;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Jobs;

[DisallowConcurrentExecution]
public class AutoRateAlbumsJob : IJob
{
    private readonly AutoRateAlbumsService _autoRateAlbumsService;
    private readonly ILogger<AutoRateAlbumsJob> _logger;
    public AutoRateAlbumsJob(AutoRateAlbumsService autoRateAlbumsService,
        ILogger<AutoRateAlbumsJob> logger)
    {
        _autoRateAlbumsService = autoRateAlbumsService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation($"Starting AutoRateAlbumsJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        await _autoRateAlbumsService.RateAlbumsAsync();
        sw.Stop();
        _logger.LogInformation($"Done AutoRateAlbumsJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}