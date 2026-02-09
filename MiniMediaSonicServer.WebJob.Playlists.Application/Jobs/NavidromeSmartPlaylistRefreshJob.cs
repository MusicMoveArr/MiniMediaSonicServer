using MiniMediaSonicServer.WebJob.Playlists.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Jobs;

[DisallowConcurrentExecution]
public class NavidromeSmartPlaylistRefreshJob : IJob
{
    private readonly NavidromeSmartPlaylistService _navidromeSmartPlaylistService;
    public NavidromeSmartPlaylistRefreshJob(
        NavidromeSmartPlaylistService navidromeSmartPlaylistService)
    {
        _navidromeSmartPlaylistService = navidromeSmartPlaylistService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        if (!Directory.Exists("/playlists"))
        {
            return;
        }

        foreach (string path in Directory.GetFiles("/playlists", "*.nsp"))
        {
            await _navidromeSmartPlaylistService.ProcessNavidromeSmartPlaylist(path);
        }
    }
}