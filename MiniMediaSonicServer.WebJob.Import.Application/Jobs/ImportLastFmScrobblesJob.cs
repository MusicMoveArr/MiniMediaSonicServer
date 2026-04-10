using System.Diagnostics;
using IF.Lastfm.Core.Api;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.WebJob.Import.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Import.Application.Jobs;

[DisallowConcurrentExecution]
public class ImportLastFmScrobblesJob : IJob
{
    private readonly ImportLastFmScrobblesService _importLastFmScrobblesService;
    public ImportLastFmScrobblesJob(ImportLastFmScrobblesService importLastFmScrobblesService)
    {
        _importLastFmScrobblesService = importLastFmScrobblesService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Starting ImportLastFmScrobblesJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        await _importLastFmScrobblesService.SyncAllUserScrobblesAsync();
        sw.Stop();
        Console.WriteLine($"Done ImportLastFmScrobblesJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}