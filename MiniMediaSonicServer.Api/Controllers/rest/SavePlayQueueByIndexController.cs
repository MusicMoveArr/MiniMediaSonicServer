using Microsoft.AspNetCore.Mvc;
using MiniMediaSonicServer.Application.Enums;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class SavePlayQueueByIndexController : SonicControllerBase
{
    private readonly UserPlayQueueService _userPlayQueueService;
    public SavePlayQueueByIndexController(UserPlayQueueService userPlayQueueService)
    {
        _userPlayQueueService = userPlayQueueService;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] SavePlayQueueByIndexRequest request)
    {
        bool success = await _userPlayQueueService.SaveUserPlayQueueByIndexAsync(request, User.UserId, User.ClientName);

        if (!success)
        {
            return SubsonicResults.Fail(HttpContext, SubsonicErrorCode.DataNotFound, "CurrentIndex is out of range");
        }
        
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse());
    }
}