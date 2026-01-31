using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class GetArtistsController : SonicControllerBase
{
    private readonly ArtistService _artistService;

    public GetArtistsController(ArtistService artistService)
    {
        _artistService = artistService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get()
    {
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse(GetUserModel())
        {
            Artists = await _artistService.GetAllArtistsAsync(User.UserId)
        });
    }
}