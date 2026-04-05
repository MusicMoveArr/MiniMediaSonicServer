using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class DeleteBookmarkController : SonicControllerBase
{
    private readonly BookmarkService _bookmarkService;
    public DeleteBookmarkController(BookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] DeleteBookmarkRequest request)
    {
        await _bookmarkService.DeleteBookmarkAsync(User.UserId, request.Id);
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}