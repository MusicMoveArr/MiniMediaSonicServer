using System.Diagnostics;
using MiniMediaSonicServer.WebJob.Scrobbler.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Scrobbler.Application.Jobs;

[DisallowConcurrentExecution]
public class ScrobblerJob : IJob
{
    private readonly ScrobblerService _scrobblerService;
    public ScrobblerJob(ScrobblerService scrobblerService)
    {
        _scrobblerService = scrobblerService;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Starting ScrobblerJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        await _scrobblerService.SendAllUserScrobblesAsync();
        sw.Stop();
        Console.WriteLine($"Done ScrobblerJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}