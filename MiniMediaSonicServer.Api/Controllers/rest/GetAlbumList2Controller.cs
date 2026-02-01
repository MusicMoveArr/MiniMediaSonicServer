using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class GetAlbumList2Controller : SonicControllerBase
{
    private readonly AlbumService _albumService;
    public GetAlbumList2Controller(AlbumService albumService)
    {
        _albumService = albumService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetAlbumList2Request request)
    {
        if (request.Size == 0)
        {
            request.Size = 50;
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse(GetUserModel())
        {
            AlbumList2 = new AlbumList2Response
            {
                Album = await _albumService.GetAlbumList2ResponseAsync(request, User.UserId)
            }
        });
    }
}