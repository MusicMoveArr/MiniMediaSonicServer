using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetTopSongsController : SonicControllerBase
{
    private readonly TrackService _trackService;
    public GetTopSongsController(TrackService trackService)
    {
        _trackService = trackService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetTopSongsRequest request)
    {
        if (request.Count <= 0)
        {
            request.Count = 20;
        }
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            TopSongs = new TopSongsResponse
            {
                Tracks = await _trackService.GetTopTracksByArtistNameAsync(request.Artist, request.Count, User.UserId)
            }
        });
    }
}