using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class GetSimilarSongsController : SonicControllerBase
{
    private readonly TrackService _trackService;
    public GetSimilarSongsController(TrackService trackService)
    {
        _trackService = trackService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetSimilarSongsRequest request)
    {
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            SimilarSongsList = new SimilarSongsListResponse
            {
                Tracks = await _trackService.GetAlbumList2ResponseAsync(request.Id, request.Count)
            }
        });
    }
}