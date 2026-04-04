using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetSongsByGenreController : SonicControllerBase
{
    private readonly TrackService _trackService;
    public GetSongsByGenreController(TrackService trackService)
    {
        _trackService = trackService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetSongsByGenreRequest request)
    {
        if (request.Count < 0)
        {
            request.Count = 10;
        }
        if (request.Count > 500)
        {
            request.Count = 500;
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            SongsByGenre = new SongsByGenreResponse
            {
                Tracks = await _trackService.GetTrackByGenreAsync(User.UserId, request.Genre, request.Count, request.Offset)
            }
        });
    }
}