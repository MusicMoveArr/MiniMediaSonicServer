using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Repositories;

namespace MiniMediaSonicServer.Application.Services;

public class StreamService
{
    private readonly StreamRepository _streamRepository;

    public StreamService(StreamRepository streamRepository)
    {
        _streamRepository = streamRepository;
    }

    public async Task<string> GetTrackPathByIdResponseAsync(Guid trackId)
    {
        return await _streamRepository.GetTrackPathByIdAsync(trackId);
    }
}