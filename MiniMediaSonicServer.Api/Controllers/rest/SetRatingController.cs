using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class SetRatingController : SonicControllerBase
{
    private readonly RatingService _ratingService;
    private readonly SearchService _searchService;
    public SetRatingController(RatingService ratingService,
        SearchService searchService)
    {
        _ratingService = ratingService;
        _searchService = searchService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] SetRatingRequest request)
    {
        ID3Type? type = await _searchService.GetID3TypeAsync(request.Id);
        switch (type)
        {
            case ID3Type.Artist:
                await _ratingService.RateArtistAsync(User.UserId, request.Id, request.Rating);
                break;
            case ID3Type.Album:
                await _ratingService.RateAlbumAsync(User.UserId, request.Id, request.Rating);
                break;
            case ID3Type.Track:
                await _ratingService.RateTrackAsync(User.UserId, request.Id, request.Rating);
                break;
        }
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}