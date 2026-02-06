using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class UnstarController : SonicControllerBase
{
    private readonly RatingService _ratingService;
    private readonly SearchService _searchService;
    public UnstarController(RatingService ratingService,
        SearchService searchService)
    {
        _ratingService = ratingService;
        _searchService = searchService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] UnstarRequest request)
    {
        if (request?.Id?.Any() == true)
        {
            foreach (Guid genericId in request.Id)
            {
                ID3Type? type = await _searchService.GetID3TypeAsync(genericId);
                switch (type)
                {
                    case ID3Type.Artist:
                        await _ratingService.StarArtistAsync(User.UserId, genericId, false);
                        break;
                    case ID3Type.Album:
                        await _ratingService.StarAlbumAsync(User.UserId, genericId, false);
                        break;
                    case ID3Type.Track:
                        await _ratingService.StarTrackAsync(User.UserId, genericId, false);
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
                    await _ratingService.StarAlbumAsync(User.UserId, genericId, false);
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
                    await _ratingService.StarArtistAsync(User.UserId, genericId, false);
                }
            }
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}