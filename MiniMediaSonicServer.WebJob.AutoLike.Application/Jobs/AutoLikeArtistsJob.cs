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
        await _autoLikeService.FavoriteArtistsAsync();
    }
}