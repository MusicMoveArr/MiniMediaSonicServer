using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class GetArtistInfo2Controller : SonicControllerBase
{
    private readonly ArtistService _artistService;
    public GetArtistInfo2Controller(ArtistService artistService)
    {
        _artistService = artistService;
    }

    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetArtistInfo2Request request)
    {
        //var artist = await _artistService.GetArtistByIdAsync(request.Id);
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            ArtistInfo2 = new ArtistInfo2
            {
                Biography = string.Empty,
                LargeImageUrl = string.Empty,
                LastFmUrl = string.Empty,
                MediumImageUrl = string.Empty,
                MusicBrainzId =  string.Empty,
                SimilarArtist = new List<ArtistID3>(),
                SmallImageUrl = string.Empty,
            }
        });
    }
}