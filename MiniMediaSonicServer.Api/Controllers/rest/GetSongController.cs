using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class GetSongController : SonicControllerBase
{
    private readonly TrackService _trackService;
    public GetSongController(TrackService trackService)
    {
        _trackService = trackService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetSongRequest request)
    {
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse(GetUserModel())
        {
            Song = await _trackService.GetTrackByIdAsync(request.Id)
        });
    }
}