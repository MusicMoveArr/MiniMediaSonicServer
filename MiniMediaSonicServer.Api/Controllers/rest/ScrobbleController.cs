using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller].view")]
public class ScrobbleController : SonicControllerBase
{
    private readonly ScrobbleService _scrobbleService;
    public ScrobbleController(ScrobbleService scrobbleService)
    {
        _scrobbleService = scrobbleService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] ScrobbleRequest request)
    {
        if (request.Submission)
        {
            await _scrobbleService.ScrobbleTrackAsync(User, request.Id, request.Time);
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}