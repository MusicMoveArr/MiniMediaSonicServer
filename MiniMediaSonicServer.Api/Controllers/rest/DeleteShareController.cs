using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class DeleteShareController : SonicControllerBase
{
    private readonly ShareService _shareService;

    public DeleteShareController(ShareService shareService)
    {
        _shareService = shareService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] DeleteShareRequest request)
    {
        await _shareService.DeleteShareAsync(User.UserId, request.Id);
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}