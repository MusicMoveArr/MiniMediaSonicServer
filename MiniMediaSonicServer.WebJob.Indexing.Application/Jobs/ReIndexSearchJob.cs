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
        await _reIndexSearchService.ReIndexSearchAsync();
    }
}