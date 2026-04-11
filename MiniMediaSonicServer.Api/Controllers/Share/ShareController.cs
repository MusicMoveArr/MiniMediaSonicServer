using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MiniMediaSonicServer.Application.Configurations;
using MiniMediaSonicServer.Application.Services;

namespace MiniMediaSonicServer.Api.Controllers.Share;

[AllowAnonymous]
[ApiController]
[Route("share")]
public class ShareController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ShareService _shareService;
    private readonly AlbumService _albumService;
    private readonly ShareConfiguration _shareConfiguration;
 
    public ShareController(IWebHostEnvironment env,
        ShareService shareService,
        AlbumService albumService,
        IOptions<ShareConfiguration> shareConfiguration)
    {
        _env = env;
        _shareService = shareService;
        _albumService = albumService;
        _shareConfiguration = shareConfiguration.Value;
    }

    [HttpGet("{shareId}")]
    [Produces("text/html")]
    public async Task<IActionResult> PlayerPage(string shareId)
    {
        var share = await _shareService.GetShareAsync(shareId);
        if (share is null ||
            await _shareService.IsExpiredAsync(share))
        {
            return NotFound();
        }

        await _shareService.IncrementVisitorCountAsync(share.ShareId);
 
        var html = await System.IO.File.ReadAllTextAsync("player.html");
 
        // Inject the share context as a JS variable before </body>
        var injection = $@"
<script>
window.__SHARE__ = {{
  id: {JsonSerializer.Serialize(share.ShareName)},
  description: {JsonSerializer.Serialize(share.Description ?? share.ShareName)},
  baseUrl: {JsonSerializer.Serialize(_shareConfiguration.BaseUrl)}
}};
</script>";
 
        html = html.Replace("<!-- __SHARE_DATA__ -->", injection);
 
        return Content(html, "text/html", Encoding.UTF8);
    }
 
    [HttpGet("{shareName}/tracks")]
    public async Task<IActionResult> GetShareTracks(string shareName)
    {
        var sharedTracks = await _shareService.GetSharedTrackAsync(shareName);
        if (!sharedTracks.Any())
        {
            return NotFound();
        }

        var tracks = sharedTracks.Select(t => new
        {
            id = t.TrackId,
            title = t.Title,
            artist = t.Artist,
            album = t.Album,
            duration = t.Duration
        });
 
        return Ok(tracks);
    }
}