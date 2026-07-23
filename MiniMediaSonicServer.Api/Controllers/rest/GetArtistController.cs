using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Repositories;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetArtistController : SonicControllerBase
{
    private readonly ArtistService _artistService;
    public GetArtistController(ArtistService artistService)
    {
        _artistService = artistService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetArtistRequest request)
    {
        var artist = await _artistService.GetArtistByIdAsync(request.Id, User.UserId);

        if (artist == null)
        {
            return SubsonicResults.Fail(HttpContext, SubsonicErrorCode.DataNotFound, "Artist not found");
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse()
        {
            Artist = artist
        });
    }
}