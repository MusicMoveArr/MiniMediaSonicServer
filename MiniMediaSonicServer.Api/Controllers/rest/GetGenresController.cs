using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class GetGenresController : SonicControllerBase
{
    private readonly TrackService _trackService;
    public GetGenresController(TrackService trackService)
    {
        _trackService = trackService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get()
    {
        var allGenres = await _trackService.GetAllGenresAsync();
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse(GetUserModel())
        {
            Genres = new Genres
            {
                Genre = allGenres.
                    Select(genre => new Genre
                    {
                        Name = genre.Genre,
                        SongCount = genre.SongCount,
                        AlbumCount = genre.AlbumCount
                    })
                    .ToList()
            }
        });
    }
}