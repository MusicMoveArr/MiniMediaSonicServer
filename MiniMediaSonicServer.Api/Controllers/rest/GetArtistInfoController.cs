using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetArtistInfoController : SonicControllerBase
{
    private readonly ArtistService _artistService;
    public GetArtistInfoController(ArtistService artistService)
    {
        _artistService = artistService;
    }

    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetArtistInfoRequest request)
    {
        var artist = await _artistService.GetLastFmArtistInfoAsync(request.Id, User.UserId, request.Count);
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            ArtistInfo = new ArtistInfo
            {
                Biography = artist?.BioContent,
                LargeImageUrl = artist?.ImageUri,
                LastFmUrl = artist?.Uri,
                MediumImageUrl = artist?.ImageUri,
                MusicBrainzId =  artist?.MusicBrainzId?.ToString(),
                SimilarArtist = artist?.SimilarArtists?.ToList() ?? [],
                SmallImageUrl = artist?.ImageUri
            }
        });
    }
}