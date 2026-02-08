using MiniMediaSonicServer.WebJob.Playlists.Application.Services;
using Quartz;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Jobs;

[DisallowConcurrentExecution]
public class FixMissingPlaylistTracksJob : IJob
{
    private readonly FixMissingPlaylistTracksService _fixMissingPlaylistTracksService;
    public FixMissingPlaylistTracksJob(FixMissingPlaylistTracksService fixMissingPlaylistTracksService)
    {
        _fixMissingPlaylistTracksService = fixMissingPlaylistTracksService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _fixMissingPlaylistTracksService.FixMissingPlaylistTracks();
    }
}