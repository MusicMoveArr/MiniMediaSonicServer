using System.Diagnostics;
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
        Console.WriteLine($"Starting NavidromeSmartPlaylistRefreshJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        if (!Directory.Exists("/playlists"))
        {
            return;
        }

        foreach (string path in Directory.GetFiles("/playlists", "*.nsp"))
        {
            await _navidromeSmartPlaylistService.ProcessNavidromeSmartPlaylist(path);
        }
        sw.Stop();
        Console.WriteLine($"Done NavidromeSmartPlaylistRefreshJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}