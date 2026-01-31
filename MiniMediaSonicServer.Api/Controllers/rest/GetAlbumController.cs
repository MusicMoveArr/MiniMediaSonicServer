using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class GetAlbumController : SonicControllerBase
{
    private readonly AlbumService _albumService;
    public GetAlbumController(AlbumService albumService)
    {
        _albumService = albumService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetAlbumRequest request)
    {
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse(GetUserModel())
        {
            Album = await _albumService.GetAlbumByIdResponseAsync(request.Id)
        });
    }
}