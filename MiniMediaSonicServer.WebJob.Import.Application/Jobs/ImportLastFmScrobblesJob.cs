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
        await _importLastFmScrobblesService.SyncAllUserScrobblesAsync();
    }
}