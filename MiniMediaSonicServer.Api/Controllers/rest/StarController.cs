using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class StarController : SonicControllerBase
{
    private readonly RatingService _ratingService;
    private readonly SearchService _searchService;
    public StarController(RatingService ratingService,
        SearchService searchService)
    {
        _ratingService = ratingService;
        _searchService = searchService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] StarRequest request)
    {
        if (request?.Id?.Any() == true)
        {
            foreach (Guid genericId in request.Id)
            {
                ID3Type? type = await _searchService.GetID3TypeAsync(genericId);
                switch (type)
                {
                    case ID3Type.Artist:
                        await _ratingService.StarArtistAsync(User.UserId, genericId, true);
                        break;
                    case ID3Type.Album:
                        await _ratingService.StarAlbumAsync(User.UserId, genericId, true);
                        break;
                    case ID3Type.Track:
                        await _ratingService.StarTrackAsync(User.UserId, genericId, true);
                        break;
                }
            }
        }

        if (request?.AlbumId?.Any() == true)
        {
            foreach (Guid genericId in request.AlbumId)
            {
                ID3Type? type = await _searchService.GetID3TypeAsync(genericId);
                if (type == ID3Type.Album)
                {
                    await _ratingService.StarAlbumAsync(User.UserId, genericId, true);
                }
            }
        }

        if (request?.ArtistId?.Any() == true)
        {
            foreach (Guid genericId in request.ArtistId)
            {
                ID3Type? type = await _searchService.GetID3TypeAsync(genericId);
                if (type == ID3Type.Artist)
                {
                    await _ratingService.StarArtistAsync(User.UserId, genericId, true);
                }
            }
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}