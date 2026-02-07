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
        if (!Directory.Exists("/playlists"))
        {
            return;
        }

        foreach (string path in Directory.GetFiles("/playlists"))
        {
            await _playlistImportService.ImportPlaylistPathAsync(path);
        }
    }
}