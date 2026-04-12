using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class UpdateShareController : SonicControllerBase
{
    private readonly ShareService _shareService;

    public UpdateShareController(ShareService shareService)
    {
        _shareService = shareService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] UpdateShareRequest request)
    {
        await _shareService.UpdateShareAsync(request.Id, User.UserId, request.Description, request.Expires);
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}