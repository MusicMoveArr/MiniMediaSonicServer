using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.Handlers.LibreFm;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.WebJob.Scrobbler.Application.Handlers.Scrobblers;

namespace MiniMediaSonicServer.Api.Controllers.mini;

[ApiController]
[Route("/mini/[controller]")]
public class ConnectLibreFmController : SonicControllerBase
{
    private readonly UserPropertyRepository _userPropertyRepository;
    private readonly LibreFmScrobbleHandler _libreFmScrobbleHandler;
    public ConnectLibreFmController(UserPropertyRepository userPropertyRepository,
        LibreFmScrobbleHandler libreFmScrobbleHandler)
    {
        _userPropertyRepository = userPropertyRepository;
        _libreFmScrobbleHandler = libreFmScrobbleHandler;
    }
    
    [HttpPost]
    public async Task<ObjectResult> Post([FromQuery] ConnectionLibreFmRequest request)
    {
        await _userPropertyRepository.SetUserPropertyAsync(User.UserId, LibreFmScrobbleHandler.LibreFmApiKeySettingName, request.LibreFmApiKey);
        await _userPropertyRepository.SetUserPropertyAsync(User.UserId, LibreFmScrobbleHandler.LibreFmApiSecretSettingName, request.LibreFmApiSecret);

        var token = await _libreFmScrobbleHandler.GetTokenAsync(request.LibreFmApiKey, request.LibreFmApiSecret);
        
        await _userPropertyRepository.SetUserPropertyAsync(User.UserId, LibreFmScrobbleHandler.LibreFmTokenSettingName, token);
        return Ok($"https://libre.fm/api/auth/?api_key={request.LibreFmApiKey}&token={token}");
    }
}