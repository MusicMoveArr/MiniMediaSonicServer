using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Entities;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
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
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse(GetUserModel())
        {
            SearchResult3 = new SearchResult3
            {
                Artists = await _searchService.SearchArtistsAsync(request.Query, request.ArtistCount, request.ArtistOffset),
                Albums = await _searchService.SearchAlbumsAsync(request.Query, request.AlbumCount, request.AlbumOffset),
                Tracks = await _searchService.SearchTracksAsync(request.Query, request.SongCount, request.SongOffset),
            }
        });
    }
}