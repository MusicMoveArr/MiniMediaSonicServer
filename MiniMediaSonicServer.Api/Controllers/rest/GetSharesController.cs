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
public class GetSharesController : SonicControllerBase
{
    private readonly ShareService _shareService;
    private readonly ShareConfiguration _shareConfiguration;

    public GetSharesController(ShareService shareService,
        IOptions<ShareConfiguration> shareConfiguration)
    {
        _shareService = shareService;
        _shareConfiguration = shareConfiguration.Value;
    }

    [HttpGet, HttpPost]
    public async Task<IResult> Get([FromQuery] SubsonicAuthModel request)
    {
        var allShares = await _shareService.GetAllSharesAsync(User.UserId);

        List<ShareResponse> sharesResponse = new List<ShareResponse>();
        foreach (var share in allShares)
        {
            var tracks = await _shareService.GetSharedTrackAsync(share.ShareName);
            sharesResponse.Add(new ShareResponse
            {
                Id = share.ShareId,
                Description = share.Description,
                Created =  share.CreatedAt,
                Url = _shareConfiguration.BaseUrl + $"/share/{share.ShareName}",
                Username = share.Username,
                VisitCount = share.VisitCount,
                Expires = share.ExpiresAt,
                LastVisited = share.LastVisitedAt,
                Tracks = tracks
            });
        }
        return SubsonicResults.Ok(HttpContext, new SubsonicResponse
        {
            Shares = new SharesResponse
            {
                Share = sharesResponse
            }
        });
    }
}