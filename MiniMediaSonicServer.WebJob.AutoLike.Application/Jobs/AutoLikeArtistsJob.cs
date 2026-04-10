using System.Diagnostics;
using MiniMediaSonicServer.WebJob.AutoLike.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.AutoLike.Application.Jobs;

[DisallowConcurrentExecution]
public class AutoLikeArtistsJob : IJob
{
    private readonly AutoLikeService _autoLikeService;
    public AutoLikeArtistsJob(AutoLikeService autoLikeService)
    {
        _autoLikeService = autoLikeService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Starting AutoLikeService at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        await _autoLikeService.FavoriteArtistsAsync();
        sw.Stop();
        Console.WriteLine($"Done AutoLikeService at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}