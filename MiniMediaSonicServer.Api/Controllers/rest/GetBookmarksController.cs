using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class GetBookmarksController : SonicControllerBase
{
    private readonly BookmarkService _bookmarkService;
    private readonly MusicCacheService _musicCacheService;
    public GetBookmarksController(BookmarkService bookmarkService,
        MusicCacheService musicCacheService)
    {
        _bookmarkService = bookmarkService;
        _musicCacheService = musicCacheService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] SubsonicAuthModel request)
    {
        var bookmarks = await _bookmarkService.GetBookmarksAsync(User.UserId);
        
        if (_musicCacheService.IsCachedFilePathExposed)
        {
            foreach (var bookmark in bookmarks.Where(b => b.Track != null))
            {
                bookmark.Track.Path = _musicCacheService.GetExposedCachedPath(bookmark.Track.Path, bookmark.Track);
            }
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse()
        {
            Bookmarks = new GetBookmarksResponse
            {
                Bookmarks = bookmarks
            }
        });
    }
}