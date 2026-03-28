using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetRandomSongsController : SonicControllerBase
{
    private readonly TrackService _trackService;
    public GetRandomSongsController(TrackService trackService)
    {
        _trackService = trackService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetRandomSongsRequest request)
    {
        if (request.Size <= 0)
        {
            request.Size = 10;
        }
        if (request.Size > 500)
        {
            request.Size = 500;
        }
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            RandomSongs = new RandomSongsResponse
            {
                Tracks = await _trackService.GetRandomTracksAsync(request.Size, User.UserId)
            }
        });
    }
}