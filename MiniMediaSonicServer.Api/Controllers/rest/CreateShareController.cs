using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Models.OpenSubsonic;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Requests;
using MiniMediaSonicServer.Application.Models.OpenSubsonic.Response;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.rest;

[ApiController]
[Route("/rest/[controller]")]
[Route("/rest/[controller].view")]
public class CreateShareController : SonicControllerBase
{
    private readonly ShareService _shareService;
    private readonly ShareConfiguration _shareConfiguration;

    public CreateShareController(ShareService shareService,
        IOptions<ShareConfiguration> shareConfiguration)
    {
        _shareService = shareService;
        _shareConfiguration = shareConfiguration.Value;
    }
    
    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] CreateShareRequest request)
    {
        if (request.Description?.Length > 1000)
        {
            request.Description = request.Description.Substring(0, 1000);
        }
        
        string shareName = await _shareService.CreateShareAsync(request.Id, User.UserId, request.Description, request.Expires);
        var share = await _shareService.GetShareAsync(shareName);
        var tracks = await _shareService.GetSharedTrackAsync(shareName);
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            Shares = new SharesResponse
            {
                Share = [
                    new ShareResponse
                    {
                        Id = share.ShareId,
                        Description = share.Description,
                        Created =  share.CreatedAt,
                        Url = _shareConfiguration.BaseUrl + $"/share/{share.ShareName}",
                        Username = User.Username,
                        VisitCount = share.VisitCount,
                        Expires = share.ExpiresAt,
                        LastVisited = share.LastVisitedAt,
                        Tracks = tracks
                    }
                ]
            }
        });
    }
}