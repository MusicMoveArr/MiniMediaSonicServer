using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class Search3Controller : SonicControllerBase
{
    private readonly SearchService _searchService;
    public Search3Controller(SearchService searchService)
    {
        _searchService = searchService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] Search3Request request)
    {
        if (request.Query == "\"\"" || request.Query == "''")
        {
            request.Query = string.Empty;
        }

        var artistsTask = _searchService.SearchArtistsAsync(request.Query, request.ArtistCount, request.ArtistOffset, User.UserId);
        var albumsTask =  _searchService.SearchAlbumsAsync(request.Query, request.AlbumCount, request.AlbumOffset, User.UserId);
        var tracksTask =  _searchService.SearchTracksAsync(request.Query, request.SongCount, request.SongOffset, User.UserId);
        await Task.WhenAll(artistsTask, albumsTask, tracksTask);
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            SearchResult3 = new SearchResult3
            {
                Artists = artistsTask.Result,
                Albums = albumsTask.Result,
                Tracks = tracksTask.Result,
            }
        });
    }
}