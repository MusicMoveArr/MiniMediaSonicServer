using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class GetSimilarSongs2Controller : SonicControllerBase
{
    private readonly TrackService _trackService;
    public GetSimilarSongs2Controller(TrackService trackService)
    {
        _trackService = trackService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetSimilarSongs2Request request)
    {
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse(GetUserModel())
        {
            SimilarSongsList2 = new SimilarSongsList2Response
                {
                    Tracks = await _trackService.GetAlbumList2ResponseAsync(request.Id, request.Count)
                }
        });
    }
}