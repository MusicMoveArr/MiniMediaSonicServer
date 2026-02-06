using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetAlbumListController : SonicControllerBase
{
    private readonly AlbumService _albumService;
    public GetAlbumListController(AlbumService albumService)
    {
        _albumService = albumService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetAlbumListRequest request)
    {
        if (request.Size == 0)
        {
            request.Size = 50;
        }

        GetAlbumList2Request request2 = new GetAlbumList2Request
        {
            Type = request.Type,
            Size = request.Size,
            Offset = request.Offset,
            FromYear = request.FromYear,
            ToYear = request.ToYear,
            Genre = request.Genre,
            MusicFolderId = request.MusicFolderId
        };
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse()
        {
            AlbumList = new AlbumListResponse
            {
                Album = await _albumService.GetAlbumList2ResponseAsync(request2, User.UserId)
            }
        });
    }
}