using System.Diagnostics;
using MiniMediaSonicServer.WebJob.Playlists.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Jobs;

[DisallowConcurrentExecution]
public class PlaylistImportJob : IJob
{
    private readonly PlaylistImportService _playlistImportService;
    public PlaylistImportJob(PlaylistImportService playlistImportService)
    {
        _playlistImportService = playlistImportService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine($"Starting PlaylistImportJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Stopwatch sw = Stopwatch.StartNew();
        if (!Directory.Exists("/playlists"))
        {
            return;
        }

        foreach (string path in Directory.GetFiles("/playlists").Where(file => !file.EndsWith(".nsp")))
        {
            await _playlistImportService.ImportPlaylistPathAsync(path);
        }
        sw.Stop();
        Console.WriteLine($"Done PlaylistImportJob at {DateTime.Now:yyyy-MM-dd HH:mm:ss}, Took {sw.Elapsed.TotalSeconds} total seconds");
    }
}