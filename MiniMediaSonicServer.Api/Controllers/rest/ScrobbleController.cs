using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
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
        if (request.Id?.Count != request.Time?.Count || 
            request.Id == null || 
            request.Time == null)
        {
            return SubsonicResults.Fail(HttpContext, SubsonicErrorCode.GenericError, "Id and Time are incorrect.");
        }
        
        for (int i = 0; i < request.Id.Count; i++)
        {
            var id = request.Id[i];
            long time = 0;
            
            if (!long.TryParse(request.Time[i],  out time))
            {
                Console.WriteLine($"Scrobble, Id='{id}' Incorrect Time, {request.Time[i]}");
                continue;
            }
            
            if (request.Submission)
            {
                await _scrobbleService.ScrobbleTrackAsync(User, id, time);
            }
            else
            {
                await _scrobbleService.PlayingNowTrackAsync(User, id, time);
            }
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}