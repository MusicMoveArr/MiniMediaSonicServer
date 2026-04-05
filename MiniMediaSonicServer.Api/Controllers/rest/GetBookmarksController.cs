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
    public GetBookmarksController(BookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] SubsonicAuthModel request)
    {
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse()
        {
            Bookmarks = new GetBookmarksResponse
            {
                Bookmarks = await _bookmarkService.GetBookmarksAsync(User.UserId)
            }
        });
    }
}