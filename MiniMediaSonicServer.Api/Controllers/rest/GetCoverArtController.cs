using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class GetCoverArtController : SonicControllerBase
{
    private static byte[] _unknownCover = System.IO.File.ReadAllBytes("./Resources/unknown_cover.png");
    private readonly CoverService _coverService;

    public GetCoverArtController(CoverService coverService)
    {
        _coverService = coverService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetCoverRequest request)
    {
        Console.WriteLine($"Grabbing cover art of '{request.Id}'");
        if (request.Id.StartsWith("album_") && Guid.TryParse(request.Id.Substring("album_".Length), out Guid albumId))
        {
            var albumCoverArt = await _coverService.GetAlbumCoverByAlbumIdAsync(albumId);
            if (albumCoverArt != null)
            {
                return Results.Bytes(albumCoverArt, "image/jpg");
            }
        }
        else if (Guid.TryParse(request.Id, out Guid genericId))
        {
            var albumCoverArt = await _coverService.GetAlbumCoverByAlbumIdAsync(genericId);
            if (albumCoverArt != null)
            {
                return Results.Bytes(albumCoverArt, "image/jpg");
            }
            
            var artistCoverArt = await _coverService.GetArtistCoverByArtistIdAsync(genericId);
            if (artistCoverArt != null)
            {
                return Results.Bytes(artistCoverArt, "image/jpg");
            }
        }

        return Results.Bytes(_unknownCover, "image/png");
    }
}