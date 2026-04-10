using System.Diagnostics;
using MiniMediaSonicServer.WebJob.Indexing.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Indexing.Application.Jobs;

[DisallowConcurrentExecution]
public class ReIndexSearchJob : IJob
{
    private readonly ReIndexSearchService _reIndexSearchService;
    public ReIndexSearchJob(ReIndexSearchService reIndexSearchService)
    {
        _reIndexSearchService = reIndexSearchService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Starting ReIndexSearchJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        await _reIndexSearchService.ReIndexSearchAsync();
        sw.Stop();
        Console.WriteLine($"Done ReIndexSearchJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}