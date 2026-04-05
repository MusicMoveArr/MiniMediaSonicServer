using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class CreateBookmarkController : SonicControllerBase
{
    private readonly BookmarkService _bookmarkService;
    public CreateBookmarkController(BookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] CreateBookmarkRequest request)
    {
        if (!await _bookmarkService.CreateBookmarkAsync(User.UserId, request.Id, request.Position, request.Comment))
        {
            return SubsonicResults.Fail(HttpContext, SubsonicErrorCode.DataNotFound, "Track not found.");
        }
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}