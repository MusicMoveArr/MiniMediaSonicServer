using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Interfaces;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetCoverArtController : SonicControllerBase
{
    private static byte[] _unknownCover = System.IO.File.ReadAllBytes("./Resources/unknown_cover.png");
    private readonly CoverService _coverService;
    private readonly string[] artistPrefix = ["ar", "artist"];
    private readonly string[] albumPrefix = ["ab", "album"];
    private const string RedisPrefixKey = "image:";
    private readonly IRedisCacheService _redisCacheService;
    private const string NotFoundCoverData = "NotFound";
    
    public GetCoverArtController(
        CoverService coverService,
        IRedisCacheService redisCacheService)
    {
        _coverService = coverService;
        _redisCacheService = redisCacheService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] GetCoverRequest request)
    {
        string extractedGuid = Regex.Match(
            request.Id, 
            "[\\w\\d]{8}-[\\w\\d]{4}-[\\w\\d]{4}-[\\w\\d]{4}-[\\w\\d]{12}",
            RegexOptions.None, 
            TimeSpan.FromMilliseconds(100))?.Value;
        
        if (Guid.TryParse(extractedGuid, out Guid genericId))
        {
            var cachedCover = await _redisCacheService.GetAsync(RedisPrefixKey, genericId.ToString());
            if (cachedCover?.Length == NotFoundCoverData.Length)
            {
                return Results.Bytes(_unknownCover, "image/png");
            }
            else if (cachedCover?.Length > 0)
            {
                return Results.Bytes(cachedCover, "image/jpg");
            }
            
            byte[]? coverArt = null;
            bool searchedArtist = false;
            bool searchedAlbum = false;
            bool searchedPlaylist = false;
            
            if(artistPrefix.Any(prefix => request.Id.StartsWith(prefix)))
            {
                coverArt = await _coverService.GetArtistCoverByArtistIdAsync(genericId);
                searchedArtist = true;
            }
            else if(albumPrefix.Any(prefix => request.Id.StartsWith(prefix)))
            {
                coverArt = await _coverService.GetAlbumCoverByAlbumIdAsync(genericId);
                searchedAlbum = true;
            }

            if (!searchedArtist && coverArt == null)
            {
                coverArt = await _coverService.GetArtistCoverByArtistIdAsync(genericId);
            }
            if (!searchedAlbum && coverArt == null)
            {
                coverArt = await _coverService.GetAlbumCoverByAlbumIdAsync(genericId);
            }
            if (!searchedPlaylist && coverArt == null)
            {
                coverArt = await _coverService.GetPlaylistCoverByIdAsync(genericId);
            }
            
            if (coverArt != null)
            {
                await _redisCacheService.SetAsync(RedisPrefixKey, extractedGuid, coverArt);
                return Results.Bytes(coverArt, "image/jpg");
            }

            await _redisCacheService.SetAsync(RedisPrefixKey, extractedGuid, Encoding.ASCII.GetBytes(NotFoundCoverData));
            return Results.Bytes(_unknownCover, "image/jpg");
        }
        
        return Results.Bytes(_unknownCover, "image/png");
    }
}